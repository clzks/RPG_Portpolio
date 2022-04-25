using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif
public class Player : MonoBehaviour, IActor
{
    private GameManager _gameManager;
    private ObjectPoolManager _objectPool;
    private DataManager _dataManager;
    private QuestManager _questManager;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private InGameCamera _camera;
    [SerializeField] private Animator _animController;
    private SphereCollider _collider;

    [Header("UI")]
    [SerializeField] private MovePad _movePad;
    [SerializeField] private ActionPad _actionPad;
    //[SerializeField] private ActionButton _attackButton;
    [SerializeField] private TargetInfoPanel _targetPanel;
    [SerializeField] private PlayerFieldStatusUI _fieldStatusUI;
    [SerializeField] private MiniMap _miniMap;
    [SerializeField] private QuestBoard _questBoard;
    [SerializeField] private DetailedQuestBoard _detailedQuestBoard;
    [SerializeField] private QuestCursor _questCursor;

    [Header("Status")]
    public float speed = 30f;
    public IActionState currActionState;
    private Dictionary<string, ActionInfo> _actionInfoList;
    private PlayerData _data { get { return _dataManager.GetPlayerData(); } set { _dataManager.SetPlayerData(value); } }
    private List<IActor> _actorList;
    private List<IBuff> _buffList;
    private int RequiredExp { get { return _data.Level * _data.Level * 5 + 30; ; } }
    
    private Dictionary<ItemType, SortedList<int,int>> _inventory { get { return _data.Inventory; } set { _data.Inventory = value; } }
    /*
        체력과 기력의 경우에는 매번 Status를 복사해오면 안되기 때문에 OriginStatus로 계산한다. 
     */
    private Status OriginStatus { get { return _data.Status; } set { _data.Status = value; } }
    private Status _equipedStatus;
    private Status _validStatus;
    private DamageInfo _damageInfo;
    private IEnumerator _moveCoroutine = null;
    private bool _followCamera = true;
    private bool _isInBattle = false;
    private int _currNormalAttackCount = 0;
    private bool _isInvincible;

    public Vector3 Position { get { return transform.position; } }
    private WaitForSeconds _buffYield;
    private float _tick;
    private void Awake()
    {
        _gameManager = GameManager.Get();
        _dataManager = DataManager.Get();
        _objectPool = ObjectPoolManager.Get();
        _questManager = QuestManager.Get();
        //_camera.SetCameraDistance(transform.position);
        currActionState = new PlayerIdleState(this);
        _actionInfoList = DataManager.Get().GetActionInfoList();
        _collider = GetComponent<SphereCollider>();
        _actorList = new List<IActor>();
        _buffList = new List<IBuff>();
        _validStatus = OriginStatus;
        _equipedStatus = new Status();
        _damageInfo = null; 
        _tick = _gameManager.tick;
        _buffYield = new WaitForSeconds(_tick);
        SetAttackButton();
        SetActionList();
        StartCoroutine(BuffUpdate());
        _questBoard.SetAction(() => { OnClickQuestBoard(); });
        _questManager.InitQuest(_dataManager.GetQuestInfoList());
        _detailedQuestBoard.SetAcceptQuestButton(OnClickAcceptQuestButton);
        _detailedQuestBoard.SetGetRewardButton(OnClickGetRewardButton);
        _detailedQuestBoard.ClearRewardIcon(_questManager.GetQuestInfo(_data.NextQuestId));
    }

    private void LateUpdate()
    {
        var array = GetEnemyPosArray();

        if (true == _followCamera)
        {
            _camera.FollowPlayer(Position);
            _miniMap.MiniMapUpdate(array, new Vector4(Position.x, Position.z, 0, 0) / 100f);
        }
    }

    private void Update()
    {
        currActionState = currActionState.Update();
        EquipStatusUpdate();
        _fieldStatusUI.UpdateStatusPanel(GetHpPercent(), GetStaminaPercent(), GetExpPercent());
        UpdateQuest();
        CheckLevelUp();
    }

    
    public void PlayAnimation(string anim)
    {
        _animController.CrossFade(anim, 0f, 0, 0f, 0.2f);
    }

    public void PlayAnimation(string anim, float startTime)
    {
        _animController.CrossFade(anim, startTime, 0, 0f, 0.2f);
    }

    public void SetForward(Vector2 dir)
    {
        transform.forward = new Vector3(dir.x, 0, dir.y);
    }

    public Vector3 GetForward()
    {
        return transform.forward;
    }

    public void MovePlayerByPad()
    {
        transform.position += _validStatus.Speed * Time.deltaTime * transform.forward;
    }

    public bool OnClickAttackButton()
    {
        return true;
    }

    public void SummonHitUnit(int index)
    {
        var state = currActionState as PlayerActionState;
        var actionInfo = state.GetActionInfo();

        if (null == actionInfo)
        {
            return;
        }

        if (actionInfo.HitUnitList.Count <= index)
        {
            return;
        }
        HitUnit hitUnit = _objectPool.MakeObject(ObjectType.HitUnit, "NormalHitUnit").GetComponent<HitUnit>();
        HitUnitInfo info = actionInfo.HitUnitList[index];
        hitUnit.SetHitUnit(this, actionInfo.DuplicatedHit, info, transform, Position);
    }

    public void TakeActor(IActor actor, HitUnitStatus hitUnit)
    {
        bool isKill = false;

        if (true == hitUnit.DuplicatedHit)
        {
            actor.TakeDamage(hitUnit, ref isKill);
        }
        else
        {
            if (false == _actorList.Contains(actor))
            {
                actor.TakeDamage(hitUnit, ref isKill);
                _actorList.Add(actor);
            }
        }

        if(true == isKill)
        {
            var info = GetCurrQuestInfo();
            int id = actor.GetId();
            AddExp(actor.GetExp());

            var expText = _objectPool.MakeObject(ObjectType.TextFloat).GetComponent<TextFloat>();
            expText.SetExpText(actor.GetExp(), Position);
            expText.ExecuteFloat();

            if (true == _questManager.IsStart() && QuestType.Kill == info.Type)
            {
                if(info.QuestTargetId == id)
                {
                    _data.CurrQuestValue += 1;
                }
            }
        }

        _targetPanel.SetTargetInfo(actor, isKill);
    }
    #region GET SET
    public Vector3 GetPosition()
    {
        return Position;
    }
    public Animator GetAnimator()
    {
        return _animController;
    }

    public MovePad GetVirtualGamePad()
    {
        return _movePad;
    }

    public ActionPad GetActionPad()
    {
        return _actionPad;
    }

    public ActionInfo GetActionInfo(string key)
    {
        if(_actionInfoList.ContainsKey(key))
        {
            return _actionInfoList[key];
        }
        else
        {
            return null;
        }
    }

    public float GetColRadius()
    {
        return _collider.radius;
    }

    public int GetId()
    {
        return -1;
    }

    public int GetExp()
    {
        return _data.Exp;
    }

    public int GetRequiredExp()
    {
        return RequiredExp;
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public ObjectType GetObjectType()
    {
        return ObjectType.Player;
    }
    public string GetName()
    {
        return "Player";
    }

    public DamageInfo GetDamageInfo()
    {
        return _damageInfo;
    }

    public float GetAttackValue()
    {
        return _validStatus.Attack;
    }

    public float GetHpPercent()
    {
        return OriginStatus.CurrHp / OriginStatus.MaxHp;
    }

    public float GetStaminaPercent()
    {
        return OriginStatus.Stamina / OriginStatus.MaxStamina;
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return _agent;
    }

    public bool GetInBattle()
    {
        return _isInBattle;
    }

    public int GetCurrNormalAttackCount()
    {
        return _currNormalAttackCount;
    }

    public Status GetValidStatus()
    {
        return _validStatus;
    }

    public Status GetOriginStatus()
    {
        return OriginStatus;
    }

    public Dictionary<ItemType, SortedList<int, int>> GetInventory()
    {
        return _inventory;
    }

    public float GetShield()
    {
        return OriginStatus.Shield;
    }
    #endregion
    public void SetAttackButton()
    {
        var action0 = _dataManager.GetActionInfo("Attack01");
        _actionPad.SetActionButton(0, action0);
    }

    public void SetActionList()
    {
        var slots = _data.SkillSlots;
        for (int i = 0; i < slots.Count; ++i)
        {
            var action = _dataManager.GetActionInfo(slots[i]);
            _actionPad.SetActionButton(i + 1, action);
            //if(slots[i] == string.Empty)
            //{
            //    continue;
            //}
            //else
            //{
            //    var action = _dataManager.GetActionInfo(slots[i]);
            //    _actionPad.SetActionButton(i + 1, action);
            //}
        }
    }

    public void SetAction(int index)
    {
        var str = _data.SkillSlots[index];

        var action = _dataManager.GetActionInfo(str);
        _actionPad.SetActionButton(index + 1, action);
    }

    

    public void Init()
    {
        
    }

    public void ReturnObject()
    {
      
    }

    public void TakeDamage(HitUnitStatus hitUnit, ref bool isDead)
    {
        //Debug.Log("플레이어에게 데미지 " + hitUnit.Damage + "만큼입힘");
        if (GetShield() > 0f)
        {
            OriginStatus.Shield -= hitUnit.Damage;
        }
        else
        {
            OriginStatus.CurrHp -= hitUnit.Damage;
        }
        // TODO 데미지 이펙트 추가할 곳
        var damageText = _objectPool.MakeObject(ObjectType.TextFloat).GetComponent<TextFloat>();
        damageText.SetText(DamageTextType.Player, (int)hitUnit.Damage, Position);
        damageText.ExecuteFloat();

        // 넉백 및 경직이 없다는 뜻
        if (0f >= hitUnit.Strength)
        {
            
        }

        if(null == _damageInfo)
        {
            _damageInfo = new DamageInfo(hitUnit.ActorPosition, hitUnit.Strength, hitUnit.Strength * 0.2f);
        }

        if(OriginStatus.CurrHp <= 0)
        {
            isDead = true;
        }
        else
        {
            isDead = false;
        }
    }

    public void ResetDamageInfo()
    {
        _damageInfo = null;
    }

    public void ResetActorList()
    {
        _actorList.Clear();
    }

    public void SetActiveNavMeshAgent(bool enabled)
    {
        _agent.enabled = enabled;
    }

    public void SetInBattle(bool enabled)
    {
        _isInBattle = enabled;
    }


    public void AddNormalAttackCount(int value)
    {
        _currNormalAttackCount += value;
    }

    public void ResetNormalAttackCount()
    {
        _currNormalAttackCount = 0;
    }

    public void MoveCharacter(float time, float distance, Vector3 dir)
    {
        if (null != _moveCoroutine)
        {
            StopCoroutine(_moveCoroutine);
        }
        _moveCoroutine = MoveCoroutine(time, distance, dir);
        StartCoroutine(_moveCoroutine);
    }

    public void MoveCharacter(float delayTime, float time, float distance, Vector3 dir)
    {
        if (null != _moveCoroutine)
        {
            StopCoroutine(_moveCoroutine);
        }
        _moveCoroutine = MoveCoroutine(delayTime, time, distance, dir);
        StartCoroutine(_moveCoroutine);
    }

    private IEnumerator MoveCoroutine(float time, float distance, Vector3 dir)
    {
        float timer = 0f;
        while (timer <= time)
        { 
            timer += Time.deltaTime;
            transform.position += (Time.deltaTime * distance / time)  * dir;
            yield return new WaitForEndOfFrame();
        }

        _moveCoroutine = null;
    }

    private IEnumerator MoveCoroutine(float delayTime, float time, float distance, Vector3 dir)
    {
        yield return new WaitForSeconds(delayTime);
        float timer = 0f;
        while (timer <= time)
        {
            timer += Time.deltaTime;
            transform.position += (Time.deltaTime * distance / time) * dir;
            yield return new WaitForEndOfFrame();
        }

        _moveCoroutine = null;
    }

    private Vector4[] GetEnemyPosArray()
    {
        Vector4[] array = new Vector4[50];
        var list = _objectPool.GetEnemies();
        list = list.FindAll(x => (x.GetPosition() - Position).magnitude <= 30f);

        for(int i = 0; i < 50; ++i)
        {
            if(list.Count - 1 < i)
            {
                array[i] = new Vector4(0, 0, 0, -100) / 100f;
                break;
            }
            else
            {
                var enemyPos = list[i].GetPosition();
                array[i] = new Vector4(enemyPos.x, enemyPos.y, enemyPos.z, 100) / 100f; 
            }
        } 

        return array;
    }

    public void EquipStatusUpdate()
    {
        _equipedStatus = Status.CopyStatus(OriginStatus);

        var list = _data.EquipmentList;

        if(null != list)
        {
            for(int i = 0; i < 3; ++i)
            {
                if(list[i] == -1)
                {
                    continue;
                }
                else
                {
                    Status stat = _dataManager.GetItemInfo(list[i]).Values;

                    _equipedStatus.Attack += stat.Attack;
                    _equipedStatus.Defence += stat.Defence;
                    _equipedStatus.Strength += stat.Strength;
                    _equipedStatus.MaxHp += stat.MaxHp;
                    _equipedStatus.Speed += stat.Speed;
                    _equipedStatus.AttackSpeed += stat.AttackSpeed;
                }
            }
        }
    }

    private IEnumerator BuffUpdate()
    {
        while (true)
        {
            _validStatus = Status.CopyStatus(_equipedStatus);

            if (null != _buffList)
            {
                if (_buffList.Count != 0)
                {
                    for(int i = 0; i < _buffList.Count; ++i)
                    {
                        _buffList[i].Update(_tick, this);
                    }
                }
            }

            yield return _buffYield;
        }
    }

    public void RemoveBuff(IBuff buff)
    {
        _buffList.Remove(buff);
    }

    public bool AddBuff(IBuff buff)
    {
        var Buff = _buffList.Find(x => x.GetId() == buff.GetId());

        if (null == Buff)
        {
            _buffList.Add(buff);
            AddBuffIcon(buff);
            return true;
        }
        else
        {
            Buff.Renew(this);
            return false;
        }
    }

    public void AddBuffIcon(IBuff buff)
    {
        var icon = _objectPool.MakeObject(ObjectType.BuffIcon).GetComponent<BuffIcon>();
        Sprite iconImage = _dataManager.GetSkillImage(buff.GetName());
        buff.SetBuffIcon(icon, iconImage);
        _fieldStatusUI.AddBuffIcon(icon);
    }

    public bool AddItem(int id)
    {
        var itemInfo = _dataManager.GetItemInfo(id);
        var Type = itemInfo.Type;
        switch (Type)
        {
            case ItemType.Weapon:
            case ItemType.Armor:
            case ItemType.Accessory:
                var set = _inventory[Type];
                if(set.Count <= 99)
                {
                    if (false == set.ContainsKey(id))
                    {
                        set.Add(id, 1);
                    }
                    else
                    {
                        set[id] += 1;
                    }
                }
                else
                {
                    Debug.Log("인벤토리가 가득 찼읍니다");
                    return false;
                }
                break;
            case ItemType.Quest:
            case ItemType.Consumable:
                var list = _inventory[Type];
                if(false == list.ContainsKey(id))
                {
                    list.Add(id, 1);
                }
                else
                {
                    if(list[id] < itemInfo.InventoryMaxCount)
                    {
                        list[id] += 1;
                    }
                    else
                    {
                        Debug.Log("아이템을 더 얻을 수 없습니다");
                        return false;
                    }
                }
                break;
        }

        return true;
    }

    public bool AddItem(int id, int count)
    {
        //if(count <= 0)
        //{
        //    Debug.Log("잘못된 접근 방식입니다");
        //    return false;
        //}

        var itemInfo = _dataManager.GetItemInfo(id);
        var Type = itemInfo.Type;
        switch (Type)
        {
            case ItemType.Weapon:
            case ItemType.Armor:
            case ItemType.Accessory:
                var set = _inventory[Type];

                if (count < 0)
                {
                    if (true == set.ContainsKey(id))
                    {
                        if (set[id] + count < 0)
                        {
                            Debug.Log("잘못된 접근 방식입니다");
                            return false;
                        }
                        else
                        {
                            set[id] += count;
                        }
                    }
                    else
                    {
                        Debug.Log("잘못된 접근 방식입니다");
                        return false;
                    }
                }
                else
                {
                    if (set.Count + count <= 100)
                    {
                        if (false == set.ContainsKey(id))
                        {
                            set.Add(id, count);
                        }
                        else
                        {
                            set[id] += count;
                        }
                    }
                    else
                    {
                        Debug.Log("인벤토리가 가득 찼읍니다");
                        return false;
                    }
                }
               
                break;
            case ItemType.Quest:
            case ItemType.Consumable:
                var list = _inventory[Type];

                if (count < 0)
                {
                    if (true == list.ContainsKey(id))
                    {
                        if(list[id] + count < 0)
                        {
                            return false;
                        }
                        else
                        {
                            list[id] += count;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (false == list.ContainsKey(id))
                    {
                        list.Add(id, count);
                    }
                    else
                    {
                        if (list[id] + count <= itemInfo.InventoryMaxCount)
                        {
                            list[id] += count;
                        }
                        else
                        {
                            Debug.Log("아이템을 더 얻을 수 없습니다");
                            return false;
                        }
                    }
                }
                break;
        }

        return true;
    }

    public int GetItemCount(int id)
    {
        int classify = id / 10000;

        ItemType type = ItemType.Count;

        switch (classify)
        {
            case 1:
                type = ItemType.Weapon;
                break;

            case 2:
                type = ItemType.Armor;
                break;

            case 3:
                type = ItemType.Accessory;
                break;

            case 4:
                type = ItemType.Quest;
                break;

            case 5:
                type = ItemType.Consumable;
                break;
        }

        if(true == _inventory[type].ContainsKey(id))
        {
            return _inventory[type][id];
        }
        else
        {
            return 0;
        }
    }

    public bool AddGold(int value)
    {
        if (_data.Gold + value < 0)
        {
            Debug.Log("골드가 부족합니다");
            return false;
        }
        else
        {
            _data.Gold += value;
            return true;
        }
    }
    public float GetStamina()
    {
        return OriginStatus.Stamina;
    }

    public void RegenStamina()
    {
        OriginStatus.Stamina += OriginStatus.RegenStamina * Time.deltaTime;

        if(OriginStatus.Stamina >= OriginStatus.MaxStamina)
        {
            OriginStatus.Stamina = OriginStatus.MaxStamina;
        }
    }

    public void AddStamina(float value)
    {
        OriginStatus.Stamina += value;
    }

    public List<int> GetEquipmentList()
    {
        return _data.EquipmentList;
    }

    public void ResetShield()
    {
        OriginStatus.Shield = 0;
    }

    public void SetFollowCamera(bool enabled)
    {
        _followCamera = enabled;
    }

    public void ResetMovePad()
    {
        _movePad.ExecuteResetMovePad();
    }

    /// <summary>
    /// 퀘스트 창 클릭
    /// </summary>
    public void OnClickQuestBoard()
    {
        _detailedQuestBoard.SetToggleOn();

        //if(false == _questManager.IsStart())
        //{
        //    StartQuest();
        //}
        //else
        //{
        //    if(true == IsSatisfyQuest())
        //    {
        //        ClearQuest();
        //    }
        //}
    }

    private void OnClickAcceptQuestButton()
    {
        StartQuest();
    }

    private void OnClickGetRewardButton()
    {
        ClearQuest();
    }

    private void StartQuest()
    {
        _questManager.StartQuest();
        var questInfo = _questManager.GetQuestInfo(_data.NextQuestId);
        if (null != questInfo)
        {
            _data.CurrQuestId = questInfo.Id;
            _data.NextQuestId = questInfo.NextQuestId;
        }
        _data.CurrQuestValue = 0;
        _detailedQuestBoard.SetDetailedBoard(questInfo);
    }
    
    private string GetSubjectText()
    {
        var questInfo = _questManager.GetQuestInfo(_data.CurrQuestId);
        string text = "";

        if (QuestType.LevelUp == questInfo.Type)
        {
            text = questInfo.Subject + "(" + _data.Level + "/" + questInfo.QuestValue + ")";
        }
        else if (QuestType.Kill == questInfo.Type)
        {
            text = questInfo.Subject + "(" + _data.CurrQuestValue + "/" + questInfo.QuestValue + ")";
        }
        else if (QuestType.Item == questInfo.Type)
        {
            text = questInfo.Subject;
        }

        return text;
    }

    public void UpdateQuest()
    {
        var questInfo = _questManager.GetQuestInfo(_data.CurrQuestId);

        if (questInfo != null)
        {
            _questBoard.UpdateText(questInfo, GetSubjectText(), IsSatisfyQuest());
        }
        else
        {
            questInfo = _questManager.GetQuestInfo(_data.NextQuestId);
        }

        if (true == _detailedQuestBoard.IsOn())
        {
            if (false == _questManager.IsStart())
            {
                _detailedQuestBoard.UpdateButtons(QuestProcessType.NoneQuest);
                _detailedQuestBoard.SetReadyDetailedBoard(questInfo);
                return;
            }

            _detailedQuestBoard.UpdateSubjectText(GetSubjectText());
            
            if(true == IsSatisfyQuest())
            {
                _detailedQuestBoard.SetClearDetailedBoard(questInfo);
                _detailedQuestBoard.UpdateButtons(QuestProcessType.SatisFy);
            }
            else
            {
                _detailedQuestBoard.UpdateButtons(QuestProcessType.Progress);
            }
        }
    }

    private void ClearQuest()
    {
        // 퀘스트 보상 확인
        var rewards = GetCurrQuestInfo().Rewards;
         
        if(null != rewards)
        {
            foreach (var reward in rewards)
            {
                switch (reward.Type)
                {
                    case RewardType.Item:
                        AddItem(reward.TargetId, reward.TargetValue);
                        break;

                    case RewardType.Exp:
                        AddExp(reward.TargetValue);
                        break;

                    case RewardType.Gold:
                        AddGold(reward.TargetValue);
                        break;

                    case RewardType.Stat:

                        break;

                    case RewardType.Count:

                        break;
                }
            }
        }

        // 퀘스트창 초기화
        _questManager.ClearQuest();
        _detailedQuestBoard.ClearRewardIcon(_questManager.GetQuestInfo(_data.NextQuestId));
        _data.CurrQuestValue = 0;
        _questBoard.UpdateText(null, "", false);
    }

    private void AddExp(int value)
    {
        _data.Exp += value;
    }

    private void CheckLevelUp()
    {
        // 임시로 필요 경험치는 Level * Level * 10 + 90으로 계산
        var level = _data.Level;

        // 레벨 업 체크
        if(_data.Exp >= RequiredExp)
        {
            LevelUp();
        }
    }

    // 레벨 업 할때 발동하는 함수
    private void LevelUp()
    {
        // 번쩍 하는 이펙트
        var info = _dataManager.GetEffectInfo("LevelUp");
        var effect = _objectPool.MakeObject(ObjectType.Effect, "LevelUp").GetComponent<BaseEffect>();
        effect.transform.position = Position;
        effect.transform.SetParent(transform);
        effect.SetEffect("LevelUp");
        effect.ExecuteEffect(info.Life);

        // 레벨업 텍스트플롯
        var levelUpText = _objectPool.MakeObject(ObjectType.TextFloat).GetComponent<TextFloat>();
        levelUpText.SetLevelUpText(Position);
        levelUpText.ExecuteFloat();

        _data.Exp -= RequiredExp;
        _data.Level += 1;
        OriginStatus.Attack += 2;
        OriginStatus.MaxHp += 10;
        OriginStatus.CurrHp = OriginStatus.MaxHp;
        OriginStatus.MaxStamina += 10;
        OriginStatus.Stamina = OriginStatus.MaxStamina;
    }

    private float GetExpPercent()
    {
        return (float)_data.Exp / RequiredExp;
    }

    private bool IsSatisfyQuest()
    {
        var currQuest = _questManager.GetQuestInfo(_data.CurrQuestId);

        if(currQuest.QuestValue <= _data.CurrQuestValue)
        {
            return true;
        }

        return false;
    }

    private QuestInfo GetCurrQuestInfo()
    {
        return _questManager.GetQuestInfo(_data.CurrQuestId);
    }

    public int GetLevel()
    {
        return _data.Level;
    }

    #region SKILL
    public void ExecuteBarrier()
    {
        var buffInfo = _dataManager.GetBuffInfo(1);
        BarrierSkill skill = new BarrierSkill(buffInfo);
        if (true == AddBuff(skill))
        {
            var effect = _objectPool.MakeObject(ObjectType.Effect, buffInfo.Name).GetComponent<BaseEffect>();
            effect.SetEffect(buffInfo.Name);
            effect.SetTargetObject(gameObject);
            effect.SetTargetPos(new Vector3(0, 1.5f, -0.7f));
            skill.SetEffect(effect);
        }
        skill.TakeActor(this);
    }

    public void ExecuteBerserk()
    {
        var buffInfo = _dataManager.GetBuffInfo(0);
        BaseBuff buff = new BaseBuff(buffInfo);
        if (true == AddBuff(buff))
        {
            var effect = _objectPool.MakeObject(ObjectType.Effect, buffInfo.Name).GetComponent<BaseEffect>();
            effect.SetEffect(buffInfo.Name);
            effect.SetPosition(Position);
            effect.SetParent(transform);
            buff.SetEffect(effect);
        }
        //buff.TakeActor(this);
    }

    public void ExecuteShockWave()
    {
        var info = _dataManager.GetEffectInfo("ShockWave");
        var life = info.Life;
        var effect = _objectPool.MakeObject(ObjectType.Effect, "ShockWave").GetComponent<BaseEffect>();
        effect.SetEffect("ShockWave", this);
        transform.rotation.ToAngleAxis(out float angle, out Vector3 axis);
        effect.SetPosition(Position);
        effect.SetRotateAround(transform.position, axis, angle);
        effect.SetInitRotation(info.IsRotate);
        effect.ExecuteEffect(life);
    }

    public void ExecuteLightningShield()
    {
        
    }

    public void ExecuteSummonSword()
    {
        var life = _dataManager.GetEffectInfo("SummonSword").Life;
        var effect = _objectPool.MakeObject(ObjectType.Effect, "SummonSword").GetComponent<BaseEffect>();
        effect.SetEffect("SummonSword", this);
        effect.SetPosition(Position + new Vector3(0, 10, 0));
        effect.ExecuteEffect(life);
    }

    public void SetInvincible(bool enabled)
    {
        _isInvincible = enabled;
    }

    public bool IsInvincible()
    {
        return _isInvincible;
    }
    #endregion


}