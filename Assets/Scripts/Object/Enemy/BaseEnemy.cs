using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif
public class BaseEnemy : MonoBehaviour, IActor
{
    public Vector3 Position { get { return transform.position; } }
    public NavMeshAgent agent;
    public Animator animator;
    private string _name;
    private IActionState currActionState;
    public Transform baseCamp;
    public Player player;
    public GameObject hitUnitPrefab;
    private List<IActor> _actorList;
    public EnemyStatus status;
    private DamageInfo _damageInfo;
    private float _currStareTimer;

    public void MakeSampleStatus()
    {
        status = new EnemyStatus();
        status.hp = 100;
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

    public void SetFoward(Vector2 dir)
    {
        transform.forward = new Vector3(dir.x, 0, dir.y);
    }
    
    public void LookPlayer()
    {
        var Pos = new Vector3(Position.x, 0, Position.z);
        var PlayerPos = new Vector3(player.Position.x, 0, player.Position.z);
        var dir = (PlayerPos - Pos).normalized;
        transform.forward = dir;
    }
    private void Awake()
    {
        MakeSampleStatus();
        _name = "TurtleShell";
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

        if (actionInfo.HitUnitList.Count <= index)
        {
            return;
        }
        HitUnit hitUnit = Instantiate(hitUnitPrefab).GetComponent<HitUnit>();
        HitUnitInfo info = actionInfo.HitUnitList[index];
        hitUnit.SetHitUnit(this, info, transform);
    }
    public void TakeActor(IActor actor, HitUnitStatus hitUnit)
    {
        if (false == _actorList.Contains(actor))
        {
            actor.TakeDamage(hitUnit);
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
     
    }

    public void ReturnObject()
    {
        
    }

    public void TakeDamage(HitUnitStatus hitUnit)
    {
        Debug.Log("플레이어에게 데미지 " + hitUnit.Damage + "만큼 입음");
        status.hp -= hitUnit.Damage;

        // 무적상태는 추후에 또 고려해봐야함
        if (null == _damageInfo && false == status.isInvincible)
        {
            _damageInfo = new DamageInfo(hitUnit.Position, hitUnit.Strength * 10);
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
}

public struct EnemyStatus
{
    public float hp;
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
