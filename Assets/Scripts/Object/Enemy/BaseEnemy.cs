using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif
public class BaseEnemy : MonoBehaviour, IActor
{
    private ObjectPoolManager _objectPool;
    public Vector3 Position { get { return transform.position; } }
    [SerializeField]private NavMeshAgent _agent;
    public Animator animator;
    private string _name;
    private IActionState currActionState;
    private Transform _baseCamp;
    private Player _player;
    //public GameObject hitUnitPrefab;
    private List<IActor> _actorList;
    public EnemyStatus status;
    private DamageInfo _damageInfo;
    private float _currStareTimer;
    private IEnumerator _moveCoroutine = null;
    public void MakeSampleStatus()
    {
        _name = "TurtleShell";
        status = new EnemyStatus();
        status.maxHp = 100;
        status.currHp = status.maxHp;
        status.chaseSpeed = 4;
        status.patrolSpeed = 2.5f;
        status.damage = 5;
        status.attackRange = 1.4f;
        status.attackTerm = 1.0f;
        status.detectionDistance = 6;
        status.chaseDistance = 8;
        status.patrolCycle = 3;
        _currStareTimer = status.attackTerm;
        _actorList = new List<IActor>();
    }

    public void SetEnemy(EnemyInfo info)
    {
        _name = info.Name;
        status = new EnemyStatus();
        status.maxHp = info.Hp;
        status.currHp = status.maxHp;
        status.chaseSpeed = info.ChaseSpeed;
        status.patrolSpeed = info.PatrolSpeed;
        status.damage = info.Damage;
        status.attackRange = info.AttackRange;
        status.attackTerm = info.AttackTerm;
        status.detectionDistance = info.DetectionDistance;
        status.chaseDistance = info.ChaseDistance;
        status.patrolCycle = info.PatrolCycle;
        _currStareTimer = status.attackTerm;
        _actorList = new List<IActor>();
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
        //MakeSampleStatus();
        _objectPool = ObjectPoolManager.Get();
        currActionState = new EnemyIdleState(this);
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
        status.currHp -= hitUnit.Damage;

        // TODO 데미지 이펙트 추가할 곳
        var damageText = _objectPool.MakeObject(ObjectType.DamageText, "DamageText").GetComponent<DamageText>();
        damageText.SetText(DamageTextType.Enemy, (int)hitUnit.Damage, Position);
        damageText.ExecuteFloat();
        // 넉백 및 경직이 없다는 뜻
        if (0f >= hitUnit.Strength)
        {
            return;
        }

        // 무적상태는 추후에 또 고려해봐야함
        if (null == _damageInfo && false == status.isInvincible)
        {
            _damageInfo = new DamageInfo(hitUnit.ActorPosition, hitUnit.Strength, hitUnit.Strength * 0.3f);
        }

        if(status.currHp <= 0)
        {
            isDead = true;
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
        return status.damage;
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
        return status.currHp / status.maxHp;
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


}

public struct EnemyStatus
{
    public float maxHp;
    public float currHp;
    public float chaseSpeed;
    public float patrolSpeed;
    public float damage;
    public float attackRange;
    public float attackTerm;

    public float detectionDistance;
    public float chaseDistance;
    public float patrolCycle;
    public bool isInvincible;
}
