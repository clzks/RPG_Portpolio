using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyActionState : IActionState
{
    protected DataManager _dataManager;
    protected BaseEnemy _enemy;
    protected Player _player;
    protected NavMeshAgent _agent;
    protected Animator _animator;
    protected Vector3 _targetPos;
    protected Transform _baseCamp;
    protected EnemyStatus _status;
    public EnemyActionState(BaseEnemy enemy)
    {
        _dataManager = DataManager.Get();
        _enemy = enemy;
        _agent = enemy.agent;
        _player = enemy.player;
        _animator = enemy.animator;
        _baseCamp = enemy.baseCamp;
        _status = enemy.status;
        Enter();
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
        if ((_enemy.Position - _player.Position).magnitude <= _status.detectionDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetPlayerDistance()
    {
        return (_enemy.Position - _player.Position).magnitude;
    }

    public void PlayAnimation(string anim)
    {
        _enemy.PlayAnimation(anim);
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
        
    }

    public override void Enter()
    {
        idleTimer = 0f;
        PlayAnimation("IdleNormal");
    }

    public override IActionState Update()
    {
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

        if (idleTimer >= _status.patrolCycle)
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

    }

    public override void Enter()
    {
        patrolTimer = 0f;
        //���̽� ķ������ ��Ʈ�� �Ÿ� �̳��� �ƹ� ��ġ�� �̵�
        _targetPos = Formula.GetRandomPatrolPosition(_baseCamp.position, 5f);
        _agent.SetDestination(_targetPos);
        _agent.speed = _status.patrolSpeed;
        PlayAnimation("Run");
    }

    public override IActionState Update()
    {
        patrolTimer += Time.deltaTime;
        _enemy.AddStareTime(Time.deltaTime);

        if (null != GetDamageInfo())
        {
            return ChangeState(new EnemyDamageState(_enemy));
        }
        // �������� �����߰ų� ��Ʈ�� �ð��� ������ Idle�� �ٲ��.
        if (patrolTimer >= 3f || true == IsArriveToDest())
        {
            return ChangeState(new EnemyIdleState(_enemy));
        }
        // ���߿� Ÿ��(�÷��̾�)�� Ž���ϸ� chase�� �ٲ��
        if (true == CheckDetectPlayer())
        {
            return ChangeState(new EnemyChaseState(_enemy));
        }

        return this;
    }

    public override void Exit()
    {

    }

    private bool IsArriveToDest()
    {
        if (_agent.pathPending == false)
        {
            if (_agent.hasPath == false || _agent.remainingDistance < 0.1f)
            {
                return true;
            }
        }

        return false;
    }
}

public class EnemyChaseState : EnemyActionState
{
    int chaseCount;
    int chaseTerm = 3;
    public EnemyChaseState(BaseEnemy enemy) : base(enemy)
    {

    }

    public override void Enter()
    {
        chaseCount = 0;
        _agent.speed = _status.chaseSpeed;
        PlayAnimation("Run");
    }

    public override IActionState Update()
    {
        _enemy.AddStareTime(Time.deltaTime);

        if (null != GetDamageInfo())
        {
            return ChangeState(new EnemyDamageState(_enemy));
        }

        if (GetPlayerDistance() >= _status.chaseDistance)
        {
            return ChangeState(new EnemyPatrolState(_enemy));
        }
        else if(GetPlayerDistance() <= _status.attackRange)
        {
            if (_enemy.GetStareTime() >= _status.attackTerm)
            {
                return ChangeState(new EnemyAttackState(_enemy));
            }
            else
            {
                return ChangeState(new EnemyStareState(_enemy));
            }
        }

        ChaseTarget(_player.Position);

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
            _agent.SetDestination(_targetPos);
        }

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
    }

    public override void Enter()
    {
        // ��θ� ���½����ش�
        _agent.ResetPath();
        // ���� ���� �� �÷��̾��� ��ġ�� �ٶ󺸸� ���� ��� ���
        _enemy.LookPlayer();
        PlayAnimation("Attack01");
    }
    public override IActionState Update()
    {
        var currAnimTime = GetAnimNormalTime(actionName);

        if (null != GetDamageInfo())
        {
            return ChangeState(new EnemyDamageState(_enemy));
        }

        // ���� �ִϸ��̼��� ���� ��Ȳ����
        if (currAnimTime >= 0.99f)
        {
            // ���� ������ ��ȯ
            return ChangeState(new EnemyStareState(_enemy));
            // ���� �÷��̾ �ʹ� ��������ٰų� �ϸ� �Ÿ��� ������ ���ϵ� ���
        }
        return this;
    }

    public override void Exit()
    {
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

    }

    public override void Enter()
    {
        _agent.ResetPath();
        // Ÿ���� �ٶ󺸰� ��
        _enemy.LookPlayer();
        PlayAnimation("Stare");
    }
    public override IActionState Update()
    {
        _enemy.AddStareTime(Time.deltaTime);

        if (null != GetDamageInfo())
        {
            return ChangeState(new EnemyDamageState(_enemy));
        }

        if (_enemy.GetStareTime() >= _status.attackTerm)
        {
            // �÷��̾ �Ÿ����� �־����ٸ� Chase�� ��ȯ
            if (GetPlayerDistance() > _status.attackRange)
            {
                return ChangeState(new EnemyChaseState(_enemy));
            }
            // ��Ÿ� �ȿ� �����ϸ� Attack���� ��ȯ
            else
            {
                return ChangeState(new EnemyAttackState(_enemy));
            }
        }
        // ��� �ð����� ������ �ְų� �ٸ� �ൿ�� �Ѵ�. ���ݰ� ���� ������ ������ ǥ���ϴ� ����
        _enemy.LookPlayer();
        return this;
    }

    public override void Exit()
    {

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

    }

    public override void Enter()
    {
        DamageInfo info = GetDamageInfo();
        knockBackTime = info.stiffNessTime;
        distance = info.distance;
        knockBackDir = (_enemy.Position - info.actorPos).normalized;
        _enemy.ResetDamageInfo();
        timer = 0f;
        // ��θ� ���½����ش�
        _agent.ResetPath();
        PlayAnimation("Damage");
        _enemy.MoveCharacter(knockBackTime, distance, knockBackDir);
    }
    public override IActionState Update()
    {
        timer += Time.deltaTime;
        _enemy.AddStareTime(Time.deltaTime);

        if (null != GetDamageInfo())
        {
            return ChangeState(new EnemyDamageState(_enemy));
        }

        if (timer >= knockBackTime)
        {
            return ChangeState(new EnemyStareState(_enemy));
        }
        // ���� �ǰ� ��Ǹ� ��� �Ŀ� chase�� ��ȯ
        return this;
    }

    public override void Exit()
    {
        
    }
}

public class EnemyStunState : EnemyActionState
{
    public EnemyStunState(BaseEnemy enemy) : base(enemy)
    {

    }

    public override void Enter()
    {
        // ��θ� ���½����ش�
        _agent.ResetPath();
        PlayAnimation("Dizzy");
    }
    public override IActionState Update()
    {
        _enemy.AddStareTime(Time.deltaTime);
        // ���� �Ŀ��� chase�� ��ȯ
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

    }

    public override void Enter()
    {
        // ��θ� ���½����ش�
        _agent.ResetPath();
        PlayAnimation("Die");
    }

    public override IActionState Update()
    {
        return this;
    }

    public override void Exit()
    {

    }
}