using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif
public class BaseEnemy : MonoBehaviour, IActor
{
    private GameManager _gameManager;
    private ObjectPoolManager _objectPool;
    private DataManager _dataManager;
    public Vector3 Position { get { return transform.position; } }
    [SerializeField]private NavMeshAgent _agent;
    public Animator animator;
    private string _name;
    private IActionState currActionState;
    private Transform _baseCamp;
    private Player _player;
    //public GameObject hitUnitPrefab;
    private List<IActor> _actorList;
    private List<IBuff> _buffList;
    private Status _originStatus;
    private Status _validStatus;
    private DamageInfo _damageInfo;
    private float _currStareTimer;
    private IEnumerator _moveCoroutine = null;
    private WaitForSeconds _buffYield;
    private float _tick;

    [Header("Drop")]
    [SerializeField] private int _exp;
    [SerializeField] private int _gold;
    [SerializeField] private List<int> _itemList;
    public void MakeSampleStatus()
    {
        _name = "TurtleShell";
        _originStatus = new Status();
        _originStatus.MaxHp = 100;
        _originStatus.CurrHp = _originStatus.MaxHp;
        _originStatus.ChaseSpeed = 4;
        _originStatus.PatrolSpeed = 2.5f;
        _originStatus.Damage = 5;
        _originStatus.AttackRange = 1.4f;
        _originStatus.AttackTerm = 1.0f;
        _originStatus.DetectionDistance = 6;
        _originStatus.ChaseDistance = 8;
        _originStatus.PatrolCycle = 3;
        _currStareTimer = _originStatus.AttackTerm;
        _actorList = new List<IActor>();
        _buffList = new List<IBuff>();
    }

    public void SetEnemy(EnemyInfo info)
    {
        _name = info.Name;
        _originStatus = new Status();
        _originStatus.CopyStatus(info.Status);
        _originStatus.CurrHp = _originStatus.MaxHp;
        //_originStatus.ChaseSpeed = info.ChaseSpeed;
        //_originStatus.PatrolSpeed = info.PatrolSpeed;
        //_originStatus.Damage = info.Damage;
        //_originStatus.AttackRange = info.AttackRange;
        //_originStatus.AttackTerm = info.AttackTerm;
        //_originStatus.DetectionDistance = info.DetectionDistance;
        //_originStatus.ChaseDistance = info.ChaseDistance;
        //_originStatus.PatrolCycle = info.PatrolCycle;
        _currStareTimer = _originStatus.AttackTerm;
        _validStatus = new Status();
        _actorList = new List<IActor>();
        _buffList = new List<IBuff>();
        SetItems(info);
        SetGold(info);
        _exp = info.Exp;

        StartCoroutine(StatusUpdate());
        currActionState = new EnemyIdleState(this);
    }

    public void SetFoward(Vector2 dir)
    {
        transform.forward = new Vector3(dir.x, 0, dir.y);
    }
    
    public void LookPlayer()
    {
        var Pos = new Vector3(Position.x, 0, Position.z);
        var PlayerPos = new Vector3(_player.Position.x, 0, _player.Position.z);
        var dir = (PlayerPos - Pos).normalized;
        transform.forward = dir;
    }
    private void OnEnable()
    {
        if(null ==_gameManager)
        {
            _gameManager = GameManager.Get();
        }

        if(null == _dataManager)
        {
            _dataManager = DataManager.Get();
        }

        if (null == _buffYield)
        {
            _tick = _gameManager.tick;
            _buffYield = new WaitForSeconds(_tick);
        }

        if (null == _objectPool)
        {
            _objectPool = ObjectPoolManager.Get();
        }
    }
    private void Update()
    {
        currActionState = currActionState.Update();
    }

    public void PlayAnimation(string anim)
    {
        animator.CrossFade(anim, 0.2f, 0, 0f, 0.2f);
    }

    public void SummonHitUnit(int index)
    {
#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name == "AnimationEditorScene")
        {
            return;
        }
#endif
        var state = currActionState as EnemyActionState;
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
        hitUnit.SetHitUnit(this, info, transform);
    }
    public void TakeActor(IActor actor, HitUnitStatus hitUnit)
    {
        bool isKill = false;

        if (false == _actorList.Contains(actor))
        {
            actor.TakeDamage(hitUnit, ref isKill);
            _actorList.Add(actor);
        }
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public ObjectType GetObjectType()
    {
        return ObjectType.Enemy;
    }

    public void Init()
    {
        //MakeSampleStatus();
    }

    public void ReturnObject()
    {
        _agent.enabled = false;
        _objectPool.ReturnObject(this);
    }

    public void SetActiveNavMeshAgent(bool enabled)
    {
        _agent.enabled = enabled;
    }

    public void TakeDamage(HitUnitStatus hitUnit, ref bool isDead)
    {
        //Debug.Log("플레이어에게 데미지 " + hitUnit.Damage + "만큼 입음");
        _originStatus.CurrHp -= hitUnit.Damage;
        if(true == _gameManager.IsOnePunchMode())
        {
            _originStatus.CurrHp = -1;
        }

        // TODO 데미지 이펙트 추가할 곳
        var damageText = _objectPool.MakeObject(ObjectType.DamageText).GetComponent<DamageText>();
        damageText.SetText(DamageTextType.Enemy, (int)hitUnit.Damage, Position);
        damageText.ExecuteFloat();
        // 넉백 및 경직이 없다는 뜻
        if (0f >= hitUnit.Strength)
        {
            return;
        }

        // 무적상태는 추후에 또 고려해봐야함
        if (null == _damageInfo && false == _originStatus.IsInvincible)
        {
            _damageInfo = new DamageInfo(hitUnit.ActorPosition, hitUnit.Strength, hitUnit.Strength * 0.3f);
        }

        // IsDead는 적군 정보 표기에 사용됨
        if (_originStatus.CurrHp <= 0)
        {
            isDead = true;
            if (0 != _itemList.Count)
            {
                foreach (var item in _itemList)
                {
                    var groundItem = _objectPool.MakeObject(ObjectType.GroundItem).GetComponent<GroundItem>();
                    groundItem.SetGroundItem(item);
                    groundItem.transform.position = transform.position;
                }
            }

            if(0 != _gold)
            {
                var groundItem = _objectPool.MakeObject(ObjectType.GroundItem).GetComponent<GroundItem>();
                groundItem.SetGroundGold(_gold);
                groundItem.transform.position = transform.position;
            }

            ReturnObject();
        }
        else
        {
            isDead = false;
        }
    }

    public void AddStareTime(float stareTime)
    {
        _currStareTimer += stareTime;
    }
    
    public void ResetStareTime()
    {
        _currStareTimer = 0f;
    }

    public float GetStareTime()
    {
        return _currStareTimer;
    }

    public DamageInfo GetDamageInfo()
    {
        return _damageInfo;
    }

    public void ResetDamageInfo()
    {
        _damageInfo = null;
    }

    public void ResetActorList()
    {
        _actorList.Clear();
    }

    #region GET SET
    public Vector3 GetPosition()
    {
        return Position;
    }

    public int GetId()
    {
        return 0;
    }

    public string GetName()
    {
        return _name;
    }

    public float GetDamage()
    {
        return _validStatus.Damage;
    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    public void SetBaseCamp(Transform baseCamp)
    {
        _baseCamp = baseCamp;
    }

    public Transform GetBaseCamp()
    {
        return _baseCamp;
    }

    public Player GetPlayer()
    {
        return _player;
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return _agent;
    }

    public float GetHpPercent()
    {
        return _originStatus.CurrHp / _validStatus.MaxHp;
    }

    public Status GetValidStatus()
    {
        return _validStatus;
    }
    #endregion
    public void MoveCharacter(float time, float distance, Vector3 dir)
    {
        if (null != _moveCoroutine)
        {
            StopCoroutine(_moveCoroutine);
        }
        _moveCoroutine = MoveCoroutine(time, distance, dir);
        StartCoroutine(_moveCoroutine);
    }

    private IEnumerator MoveCoroutine(float time, float distance, Vector3 dir)
    {
        float timer = 0f;
        while (timer <= time)
        {
            timer += Time.deltaTime;
            transform.position += (Time.deltaTime * distance / time) * dir;
            yield return new WaitForEndOfFrame();
        }

        _moveCoroutine = null;
    }

    private IEnumerator StatusUpdate()
    {
        while(true)
        {
            _validStatus.CopyStatus(_originStatus);


            if (null != _buffList)
            {
                foreach (var buff in _buffList)
                {
                    buff.Update(_tick, this);
                }
            }
            yield return _buffYield;
        }
    }

    public void RemoveBuff(IBuff buff)
    {
        _buffList.Remove(buff);
    }

    private void SetGold(EnemyInfo info)
    {
        bool isDrop = info.DropGoldPercentage > UnityEngine.Random.Range(0, 1f);

        if(false == isDrop)
        {
            return;
        }

        _gold = UnityEngine.Random.Range(info.MinGold, info.MaxGold);
    }

    private void SetItems(EnemyInfo info)
    {
        if (null == info.DropItemList)
        {
            return;
        }
        else
        {
            if (0 == info.DropItemList.Count)
            {
                return;
            }
        }

        for (int i = 0; i < info.DropItemList.Count; ++i)
        {
            bool isDrop = info.DropItemPercentage > UnityEngine.Random.Range(0, 1f);

            if (false == isDrop)
            {
                continue;
            }

            _itemList.Add(info.DropItemList[i]);
        }
    }
}