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
    protected GameManager _gameManager;
    protected ObjectPoolManager _objectPool;
    protected DataManager _dataManager;
    public Vector3 Position { get { return transform.position; } }
    [SerializeField]private NavMeshAgent _agent;
    [SerializeField] private Transform _rootTransform;
    [SerializeField] private SkinnedMeshRenderer _renderer;
    public Vector3 RootPosition { get { return _rootTransform.position; } }
    public Animator animator;
    protected string _name;
    protected int _id;
    protected IActionState currActionState;
    protected Transform _baseCamp;
    protected Player _player;
    //public GameObject hitUnitPrefab;
    
    protected DamageInfo _damageInfo;
    protected float _currStareTimer;
    protected IEnumerator _moveCoroutine = null;
    protected WaitForSeconds _buffYield;
    protected float _tick;

    [Header("Status")]
    protected List<IActor> _actorList;
    protected List<IBuff> _buffList;
    protected Status _originStatus;
    protected Status _validStatus;
    protected bool _isInvincible = false;
    private float _hitTimer;        // 피격시 피격효과 타이머
    private float _hitTime = 0.1f;  // 피격효과 시간
    [Header("Drop")]
    [SerializeField] protected int _exp;
    [SerializeField] protected int _gold;
    [SerializeField] protected List<int> _itemList;
    public void MakeSampleStatus()
    {
        _name = "TurtleShell";
        _originStatus = new Status();
        _originStatus.MaxHp = 100;
        _originStatus.CurrHp = _originStatus.MaxHp;
        _originStatus.ChaseSpeed = 4;
        _originStatus.PatrolSpeed = 2.5f;
        _originStatus.Attack = 5;
        _originStatus.AttackRange = 1.4f;
        _originStatus.AttackTerm = 1.0f;
        _originStatus.DetectionDistance = 6;
        _originStatus.ChaseDistance = 8;
        _originStatus.PatrolCycle = 3;
        _currStareTimer = _originStatus.AttackTerm;
        _actorList = new List<IActor>();
        _buffList = new List<IBuff>();
    }
    private void Awake()
    {
        DonDestroy();
    }
    public virtual void SetEnemy(EnemyInfo info, IActionState actionState)
    {
        _name = info.Name;
        _id = info.Id;
        _originStatus = new Status();
        _originStatus = Status.CopyStatus(info.Status);
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
        _isInvincible = false;
        StartCoroutine(StatusUpdate());
        currActionState = actionState;
    }

    public void SetFoward(Vector2 dir)
    {
        transform.forward = new Vector3(dir.x, 0, dir.y);
    }

    /// <summary>
    /// 특정 상태에서 플레이어를 바라보게 하는 함수
    /// </summary>
    /// <param name="isImmediate">true : 곧바로 바라봄, false : 보간의 형태로 바라봄</param>
    /// <param name="speed">보간이 일어나는 속도 (true일 경우 사용안함)</param>
    public virtual void LookPlayer(bool isImmediate = true, float speed = 0.3f)
    {
        var Pos = new Vector3(Position.x, 0, Position.z);
        var PlayerPos = new Vector3(_player.Position.x, 0, _player.Position.z);
        var dir = (PlayerPos - Pos).normalized;
        var currDir = transform.forward;
        
        if (true == isImmediate)
        {
            transform.forward = dir;
        }
        else
        {
            Vector3 interpolation = Vector3.Lerp(currDir, dir, speed);
            transform.forward = interpolation;
        }
    }
    protected virtual void OnEnable()
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
    protected virtual void Update()
    {
        currActionState = currActionState.Update();
        UpdateHitTimer();
    }

    public virtual void PlayAnimation(string anim, bool isCrossFade = true)
    {
        if (true == isCrossFade)
        {
            animator.CrossFade(anim, 0.2f, 0, 0f, 0.2f);
        }
        else
        {
            animator.Play(anim);
        }
    }

    // ActionInfo 파일에 있는 정보를 토대로 시전자의 위치와 방향에 기반한 히트유닛 생성방법
    public virtual void SummonHitUnit(int index)
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
        hitUnit.SetHitUnit(this, false, info, transform, RootPosition);
    }

    // 시전자의 위치 및 방향에게서 자유롭게 히트유닛을 생성하기 위한 함수
    public virtual HitUnit MakeHitUnit(EnemyAction actionInfo = null)
    {
        var state = currActionState as EnemyActionState;

        if (null == actionInfo)
        {
            actionInfo = state.GetActionInfo();
        }

        HitUnit hitUnit = _objectPool.MakeObject(ObjectType.HitUnit, "NormalHitUnit").GetComponent<HitUnit>();
        HitUnitInfo info = actionInfo.HitUnitList[0];
        hitUnit.SetHitUnit(this, true, info);

        return hitUnit;
    }

    public virtual void TakeActor(IActor actor, HitUnitStatus hitUnit)
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
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public ObjectType GetObjectType()
    {
        return ObjectType.Enemy;
    }

    public virtual void Init()
    {
        //MakeSampleStatus();
    }

    public virtual void ReturnObject()
    {
        _agent.enabled = false;
        _objectPool.ReturnObject(this);
    }

    public virtual void SetActiveNavMeshAgent(bool enabled)
    {
        _agent.enabled = enabled;
    }

    public virtual void TakeDamage(HitUnitStatus hitUnit, ref bool isDead)
    {
        //Debug.Log("플레이어에게 데미지 " + hitUnit.Damage + "만큼 입음");
        _originStatus.CurrHp -= hitUnit.Damage;
        if(true == _gameManager.IsOnePunchMode())
        {
            _originStatus.CurrHp = -1;
        }

        // TODO 데미지 이펙트 추가할 곳
        var damageText = _objectPool.MakeObject(ObjectType.TextFloat).GetComponent<TextFloat>();
        damageText.SetText(DamageTextType.Enemy, (int)hitUnit.Damage, RootPosition);
        damageText.ExecuteFloat();

        // 피격시 하얗게 변하는 효과
        if (null != _renderer)
        {
            _renderer.material.SetInt("isHit", 1);
            _hitTimer = _hitTime;
        }

        // 넉백 및 경직이 없다는 뜻
        if (0f >= hitUnit.Strength)
        {
            return;
        }

        // 무적상태는 추후에 또 고려해봐야함
        if (null == _damageInfo)
        {
            _damageInfo = new DamageInfo(hitUnit.ActorPosition, hitUnit.Strength, hitUnit.Strength * 0.3f);
        }

        // IsDead는 적군 정보 표기에 사용됨
        if (_originStatus.CurrHp <= 0)
        {
            isDead = true;
        }
        else
        {
            isDead = false;
        }
    }

    public void UpdateHitTimer()
    {
        _hitTimer -= Time.deltaTime;

        if (_hitTimer <= 0f)
        {
            _hitTimer = 0f;
            _renderer.material.SetInt("isHit", 0);
        }
    }

    public void ExecuteDead()
    {
        if (0 != _itemList.Count)
        {
            foreach (var item in _itemList)
            {
                var groundItem = _objectPool.MakeObject(ObjectType.GroundItem).GetComponent<GroundItem>();
                groundItem.SetGroundItem(item);
                groundItem.transform.position = transform.position;
            }
        }

        if (0 != _gold)
        {
            var groundItem = _objectPool.MakeObject(ObjectType.GroundItem).GetComponent<GroundItem>();
            groundItem.SetGroundGold(_gold);
            groundItem.transform.position = transform.position;
        }

        ReturnObject();
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
        return _id;
    }

    public int GetExp()
    {
        return _exp;
    }
    public string GetName()
    {
        return _name;
    }

    public float GetAttackValue()
    {
        return _validStatus.Attack;
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

    public bool IsZeroHp()
    {
        if(_originStatus.CurrHp <= 0)
        {
            return true;
        }

        return false;
    }
    
    public Status GetValidStatus()
    {
        return _validStatus;
    }

    public Status GetOriginStatus()
    {
        return _originStatus;
    }

    public float GetShield()
    {
        return _originStatus.Shield;
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
            _validStatus = Status.CopyStatus(_originStatus);

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

    public bool AddBuff(IBuff buff)
    {
        var Buff = _buffList.Find(x => x.GetId() == buff.GetId());

        if (null == Buff)
        {
            _buffList.Add(buff);
            return true;
        }
        else
        {
            Buff.Renew(this);
            return false;
        }
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

    public void ResetShield()
    {
        _originStatus.Shield = 0;
    }


    public bool IsInvincible()
    {
        return _isInvincible;
    }

    public void SetInvincible(bool enabled)
    {
        _isInvincible = enabled;
    }

    public Transform GetRootTransform()
    {
        return _rootTransform;
    }

    public void DonDestroy()
    {
        DontDestroyOnLoad(gameObject);
    }
}