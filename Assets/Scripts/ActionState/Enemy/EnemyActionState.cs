using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyActionState : IActionState
{
    protected DataManager _dataManager;
    protected BaseEnemy _enemy;
    protected NavMeshAgent Agent { get { return _enemy.GetNavMeshAgent(); } }
    protected Animator _animator;
    protected Vector3 _targetPos;
    protected Status _status { get { return _enemy.GetValidStatus(); } }
    public EnemyActionState(IActor enemy)
    {
        _dataManager = DataManager.Get();
        _enemy = (BaseEnemy)enemy;
        _animator = _enemy.animator;
    }

    public abstract void Enter();
    public abstract IActionState Update();
    public abstract void Exit();
    public IActionState ChangeState(IActionState state)
    {
        Exit();
        return state;
    }

    public virtual bool CheckDetectPlayer()
    {
        if ((_enemy.Position - _enemy.GetPlayer().Position).magnitude <= _status.DetectionDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetBaseDistance()
    {
        return (_enemy.Position - _enemy.GetBaseCamp().position).magnitude;
    }

    public float GetPlayerDistance()
    {
        return (_enemy.Position - _enemy.GetPlayer().Position).magnitude;
    }

    public Vector3 GetPlayerDir()
    {
        return (_enemy.GetPlayer().Position - _enemy.Position).normalized;
    }

    public void PlayAnimation(string anim, bool isCrossFade = true)
    {
        _enemy.PlayAnimation(anim, isCrossFade);
    }

    public DamageInfo GetDamageInfo()
    {
        return _enemy.GetDamageInfo();
    }

    public float GetAnimNormalTime(string anim)
    {
        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float currAnimTime = 0f;
        if (stateInfo.IsName(anim))
        {
            currAnimTime = stateInfo.normalizedTime;
        }

        return currAnimTime;
    }

    public float GetAnimTotalTime(string anim)
    {
        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float currAnimTime = 0f;
        if (stateInfo.IsName(anim))
        {
            currAnimTime = stateInfo.length / stateInfo.speed;
        }

        return currAnimTime;
    }

    public virtual EnemyAction GetActionInfo()
    {
        return null;
    }

    protected bool IsArriveToDest()
    {
        if (Agent.pathPending == false)
        {
            if (Agent.hasPath == false || Agent.remainingDistance < 0.1f)
            {
                return true;
            }
        }

        return false;
    }

    #region sealed methods
    public sealed override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public sealed override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public sealed override string ToString()
    {
        return base.ToString();
    }
    #endregion
}

public class EnemyIdleState : EnemyActionState
{
    float idleTimer;

    public EnemyIdleState(BaseEnemy enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        idleTimer = 0f;
        PlayAnimation("IdleNormal");
    }

    public override IActionState Update()
    {
        if(true == _enemy.IsZeroHp())
        {
            return new EnemyDieState(_enemy);
        }

        idleTimer += Time.deltaTime;
        _enemy.AddStareTime(Time.deltaTime);

        if (null != GetDamageInfo())
        {
            return ChangeState(new EnemyDamageState(_enemy));
        }

        if (true == CheckDetectPlayer())
        {
            return ChangeState(new EnemyChaseState(_enemy));
        }

        if (idleTimer >= _status.PatrolCycle)
        {
            return ChangeState(new EnemyPatrolState(_enemy));
        }
        else
        {
            return this;
        }
    }

    public override void Exit()
    {
        
    }

}

public class EnemyPatrolState : EnemyActionState
{
    float patrolTimer;

    public EnemyPatrolState(BaseEnemy enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        patrolTimer = 0f;
        //베이스 캠프에서 패트롤 거리 이내의 아무 위치로 이동
        _targetPos = Formula.GetRandomPatrolPosition(_enemy.GetBaseCamp().position, 5f);
        Agent.SetDestination(_targetPos);
        Agent.speed = _status.PatrolSpeed;
        PlayAnimation("Run");
    }

    public override IActionState Update()
    {
        if (true == _enemy.IsZeroHp())
        {
            return new EnemyDieState(_enemy);
        }

        patrolTimer += Time.deltaTime;
        _enemy.AddStareTime(Time.deltaTime);

        if (null != GetDamageInfo())
        {
            return ChangeState(new EnemyDamageState(_enemy));
        }
        // 목적지에 도달했거나 패트롤 시간이 지나면 Idle로 바뀐다.
        if (patrolTimer >= 3f || true == IsArriveToDest())
        {
            return ChangeState(new EnemyIdleState(_enemy));
        }
        // 도중에 타깃(플레이어)를 탐지하면 chase로 바뀐다
        if (true == CheckDetectPlayer())
        {
            return ChangeState(new EnemyChaseState(_enemy));
        }

        return this;
    }

    public override void Exit()
    {

    }
}

public class EnemyChaseState : EnemyActionState
{
    int chaseCount;
    int chaseTerm = 3;
    public EnemyChaseState(BaseEnemy enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        chaseCount = 0;
        Agent.speed = _status.ChaseSpeed;
        PlayAnimation("Run");
    }

    public override IActionState Update()
    {
        if (true == _enemy.IsZeroHp())
        {
            return new EnemyDieState(_enemy);
        }

        _enemy.AddStareTime(Time.deltaTime);

        if (null != GetDamageInfo())
        {
            return ChangeState(new EnemyDamageState(_enemy));
        }

        if (GetPlayerDistance() >= _status.ChaseDistance)
        {
            return ChangeState(new EnemyPatrolState(_enemy));
        }
        else if(GetPlayerDistance() <= _status.AttackRange)
        {
            if (_enemy.GetStareTime() >= _status.AttackTerm)
            {
                return ChangeState(new EnemyAttackState(_enemy));
            }
            else
            {
                return ChangeState(new EnemyStareState(_enemy));
            }
        }

        ChaseTarget(_enemy.GetPlayer().Position);

        return this;
    }

    public override void Exit()
    {

    }

    private void ChaseTarget(Vector3 pos)
    {
        if (chaseCount == 0)
        {
            _targetPos = pos;
            Agent.SetDestination(_targetPos);
        }

        // 매 프레임마다 SetDestination을 호출하는게 신경쓰여서 3프레임에 한번씩 호출되도록 함
        chaseCount++;

        if (chaseCount > chaseTerm - 1)
        {
            chaseCount = 0;
        }
    }
}

public class EnemyAttackState : EnemyActionState
{
    string actionName = "Attack01";
    EnemyAction info;

    public EnemyAttackState(BaseEnemy enemy) : base(enemy)
    {
        info = _dataManager.GetEnemyActionInfo(_enemy.GetName(), actionName);

        if (null == info)
        {

        }

        Enter();
    }

    public override void Enter()
    {
        Agent.avoidancePriority = 30;
        // 경로를 리셋시켜준다
        Agent.ResetPath();
        // 공격 시작 시 플레이어의 위치를 바라보며 공격 모션 재생
        _enemy.LookPlayer();
        PlayAnimation(actionName);
    }
    public override IActionState Update()
    {
        if (true == _enemy.IsZeroHp())
        {
            return new EnemyDieState(_enemy);
        }

        var currAnimTime = GetAnimNormalTime(actionName);

        if (null != GetDamageInfo())
        {
            return ChangeState(new EnemyDamageState(_enemy));
        }

        // 공격 애니메이션이 끝난 상황에서
        if (currAnimTime >= 0.99f)
        {
            // 공격 대기모드로 변환
            return ChangeState(new EnemyStareState(_enemy));
            // 만약 플레이어가 너무 가까워졌다거나 하면 거리를 벌리는 패턴도 고려
        }
        return this;
    }

    public override void Exit()
    {
        Agent.avoidancePriority = 50;
        _enemy.ResetStareTime();
        _enemy.ResetActorList();
    }

    public override EnemyAction GetActionInfo()
    {
        return info;
    }
}

public class EnemyStareState : EnemyActionState
{
    
    public EnemyStareState(BaseEnemy enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        Agent.avoidancePriority = 30;
        Agent.ResetPath();
        // 타깃을 바라보게 함
        _enemy.LookPlayer();
        PlayAnimation("Stare");
    }
    public override IActionState Update()
    {
        if (true == _enemy.IsZeroHp())
        {
            return new EnemyDieState(_enemy);
        }

        _enemy.AddStareTime(Time.deltaTime);

        if (null != GetDamageInfo())
        {
            return ChangeState(new EnemyDamageState(_enemy));
        }

        if (_enemy.GetStareTime() >= _status.AttackTerm)
        {
            // 플레이어가 거리에서 멀어졌다면 Chase로 변환
            if (GetPlayerDistance() > _status.AttackRange)
            {
                return ChangeState(new EnemyChaseState(_enemy));
            }
            // 사거리 안에 존재하면 Attack으로 변환
            else
            {
                return ChangeState(new EnemyAttackState(_enemy));
            }
        }
        // 대기 시간동안 가만히 있거나 다른 행동을 한다. 공격과 공격 사이의 간격을 표현하는 상태
        _enemy.LookPlayer();
        return this;
    }

    public override void Exit()
    {
        Agent.avoidancePriority = 50;
    }
}

public class EnemyDamageState : EnemyActionState
{
    float knockBackTime;
    float timer;
    float distance;
    Vector3 knockBackDir;

    public EnemyDamageState(BaseEnemy enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        DamageInfo info = GetDamageInfo();
        knockBackTime = info.stiffNessTime;
        distance = info.distance;
        knockBackDir = (_enemy.Position - info.actorPos).normalized;
        _enemy.ResetDamageInfo();
        timer = 0f;
        // 경로를 리셋시켜준다
        Agent.ResetPath();
        PlayAnimation("Damage");
        _enemy.MoveCharacter(knockBackTime, distance, knockBackDir);
    }
    public override IActionState Update()
    {
        if (true == _enemy.IsZeroHp())
        {
            return new EnemyDieState(_enemy);
        }

        timer += Time.deltaTime;
        //_enemy.AddStareTime(Time.deltaTime);

        if (null != GetDamageInfo())
        {
            return ChangeState(new EnemyDamageState(_enemy));
        }

        if (timer >= knockBackTime)
        {
            return ChangeState(new EnemyStareState(_enemy));
        }
        // 단지 피격 모션만 재생 후에 chase로 귀환
        return this;
    }

    public override void Exit()
    {
        _enemy.ResetStareTime();
    }
}

public class EnemyStunState : EnemyActionState
{
    public EnemyStunState(BaseEnemy enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        // 경로를 리셋시켜준다
        Agent.ResetPath();
        PlayAnimation("Dizzy");
    }
    public override IActionState Update()
    {
        if (true == _enemy.IsZeroHp())
        {
            return new EnemyDieState(_enemy);
        }

        _enemy.AddStareTime(Time.deltaTime);
        // 스턴 후에는 chase로 귀환
        return this;
    }

    public override void Exit()
    {
         
    }
}

public class EnemyDieState : EnemyActionState
{
    public EnemyDieState(BaseEnemy enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        // 사망 애니메이션 중에 추가 피격을 당하지 않게 에이전트를 꺼주면서 무적처리를 해준다.
        Agent.ResetPath();
        _enemy.SetInvincible(true);
        _enemy.SetActiveNavMeshAgent(false);
        // 경로를 리셋시켜준다
        PlayAnimation("Die");
    }

    public override IActionState Update()
    {
        var currAnimTime = GetAnimNormalTime("Die");

        if(currAnimTime >= 0.99f)
        {
            _enemy.ExecuteDead();
        }

        return this;
    }

    public override void Exit()
    {
    
    }
}