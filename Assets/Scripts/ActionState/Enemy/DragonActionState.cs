using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class DragonActionState : EnemyActionState
{
    protected Dragon _dragon;

    protected float minNormalAttackRange = 3.5f;
    protected float maxNormalAttackRange = 5.5f;

    protected float minDashAttackRange = 6.0f;
    protected float maxDashAttackRange = 8.5f;

    protected bool _isTriggerOn = false;
    protected float _triggerTime = -1f;

    public DragonActionState(IActor enemy) : base (enemy)
    {
        _dataManager = DataManager.Get();
        _dragon = (Dragon)enemy;
        _animator = _enemy.animator;

        SetDragonSpeed();
    }

    public abstract override void Enter();
    
    public abstract override void Exit();

    public abstract override IActionState Update();

    protected bool OnTrigger(float time)
    {
        if(_triggerTime <= 0f)
        {
            return false;
        }

        if(_triggerTime <= time && false == _isTriggerOn)
        {
            _isTriggerOn = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void ResetNormalTimer()
    {
        _dragon.ResetNormalTimer();
    }

    protected void ResetDashTimer()
    {
        _dragon.ResetDashTimer();
    }

    protected void ResetFireTimer()
    {
        _dragon.ResetFireTimer();
    }

    protected void UpdateAttackTimer()
    {
        _dragon.UpdateAttackTimer();
    }

    protected void SetDragonSpeed()
    {
        _dragon.GetNavMeshAgent().angularSpeed = 120f;

        //if (false == _dragon.IsFrenzy())
        //{
        //    _animator.speed = 1f;
        //    _dragon.GetNavMeshAgent().speed = 6f;
        //}
        //else
        //{
        //    _animator.speed = 1.5f;
        //    _dragon.GetNavMeshAgent().speed = 9f;
        //}

        _animator.speed = 1f;
        _dragon.GetNavMeshAgent().speed = 10f;
    }

    protected bool CheckDifficultyChance()
    {
        int diff = _dragon.GetDragonProDiff();
        int curr = _dragon.GetDragonCurrDiff();
        
        if(curr < diff)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}


public class DragonGazeState : DragonActionState
{
    float _timer = 0f;

    public DragonGazeState(IActor enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        Agent.ResetPath();
        PlayAnimation("Idle");
    }

    public override void Exit()
    {
        
    }

    public override IActionState Update()
    {
        _timer += Time.deltaTime;

        if(_timer <= 1f)
        {
            return this;
        }

        UpdateAttackTimer();

        float distance = GetPlayerDistance();

        if(true == CheckDifficultyChance())
        {
            _dragon.ChangeDifficulty();
            
        }

        // �þ߿� �÷��̾ ������ ���
        if (true == Formula.IsTargetInSight(_dragon.transform.forward, 30f, _dragon.GetPosition(), _dragon.GetPlayer().Position))
        {
            // �ſ� ������ ���� ���
            if(distance < minNormalAttackRange)
            {
                return ChangeState(new DragonTakeOffState(_dragon));
            }
            // �Ϲݰ��� ��Ÿ� ���� ��ġ�� ��
            else if (distance >= minNormalAttackRange && distance <= maxNormalAttackRange)
            {
                if (_dragon.GetNormalTimer() <= 0f)
                {
                    return ChangeState(new DragonNormalAttackState(_dragon));
                }
            }
            // �� �ָ� ������
            else
            {
                // �뽬 ���� ��Ÿ�� ������ ���
                if (_dragon.GetDashTimer() <= 0f)
                {
                    if (distance >= minDashAttackRange && distance <= maxDashAttackRange)
                    {
                        return ChangeState(new DragonDashAttackState(_dragon));
                    }
                }
                else
                {
                    return ChangeState(new DragonChaseState(_dragon));
                }
            }
        }
        // �ƴҰ��
        else
        {
            return ChangeState(new DragonChaseState(_dragon));
        }

        return this;
    }
}

public class DragonChaseState : DragonActionState
{
    public DragonChaseState(IActor enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        PlayAnimation("Run");
        //if (false == _dragon.IsFrenzy())
        //{
        //    PlayAnimation("Walk");
        //}
        //else
        //{
        //    PlayAnimation("Run");
        //}
    }

    public override void Exit()
    {

    }

    public override IActionState Update()
    {
        float distance = GetPlayerDistance();

        UpdateAttackTimer();

        if (_dragon.GetFireTimer() <= 0f)
        {
            //return ChangeState(new DragonTakeOffState(_dragon));
        }

        // ���� �þ� 30�� ���� �÷��̾ ��ġ���� ���
        if (true == Formula.IsTargetInSight(_dragon.transform.forward, 30f, _dragon.GetPosition(), _dragon.GetPlayer().Position))
        {
            SetDragonSpeed();

            // �ſ� ������ ���� ���
            if (distance < minNormalAttackRange)
            {
                if (_dragon.GetNormalTimer() <= 0f)
                {
                    return ChangeState(new DragonNormalAttackState(_dragon));
                }
                else
                {
                    return ChangeState(new DragonGazeState(_dragon));
                }
            }
            // �Ϲݰ��� ��Ÿ� ���� ��ġ�� ��
            else if (distance >= minNormalAttackRange && distance <= maxNormalAttackRange)
            {
                if (_dragon.GetNormalTimer() <= 0f)
                {
                    return ChangeState(new DragonNormalAttackState(_dragon));
                }
                else
                {
                    return ChangeState(new DragonGazeState(_dragon));
                }
            }
            // �� �ָ� ���� ���
            else
            {
                // �뽬 ���� ��Ÿ�� ������ ���
                if (_dragon.GetDashTimer() <= 0f)
                {
                    if (distance >= minDashAttackRange && distance <= maxDashAttackRange)
                    {
                        return ChangeState(new DragonDashAttackState(_dragon));
                    }
                }
            }
        }
        // �þ� ���� ���� ���
        else
        {
            //_dragon.GetNavMeshAgent().speed = 0.2f;
            //_dragon.GetNavMeshAgent().angularSpeed = 180f;
            _dragon.LookPlayer(false, 0.016f);

            // Burst ��Ÿ���� �� ���Ұ�, Burst ��Ÿ����� ���� ��� Burst
            if (_dragon.GetBurstTimer() <= 0f)
            {
                return ChangeState(new DragonBurstAttackState(_dragon));
            }
            // ��Ÿ���� �� �ȵ����� ��� ��Ÿ���� ����. (Burst�� Chase ������ ��쿡�� ��Ÿ���� ���ư���.
            else
            {
                _dragon.UpdateBurstTimer();
            }
        }

        ChaseTarget(_dragon.GetPlayer().Position);

        return this;
    }

    private void ChaseTarget(Vector3 pos)
    {
        _targetPos = pos;
        Agent.SetDestination(_targetPos);
    }
}

public class DragonBurstAttackState : DragonActionState
{
    string _actionName = "DragonBurst";

    EnemyAction info;

    public DragonBurstAttackState(IActor enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        _triggerTime = 0.29f;
        // �⺻���� Ÿ�̸� �ʱ�ȭ
        _dragon.ResetNormalTimer();

        info = _dataManager.GetEnemyActionInfo(_enemy.GetName(), _actionName);
        Agent.avoidancePriority = 30;
        // ��θ� ���½����ش�
        Agent.ResetPath();
        // ���� ���� �� �÷��̾��� ��ġ�� �ٶ󺸸� ���� ��� ���
        //_enemy.LookPlayer();
        _animator.speed *= 1.5f;
        PlayAnimation(_actionName);
    }

    public override void Exit() 
    {
        _dragon.ResetActorList();
        _dragon.ResetBurstTimer();
    }

    public override IActionState Update()
    {
        var currAnimTime = GetAnimNormalTime(_actionName);

        if(true == OnTrigger(currAnimTime))
        {
            _dragon.ExecuteBurstAttack();
        }

        if (currAnimTime >= 0.99f)
        {
            return ChangeState(new DragonGazeState(_dragon));
        }

        return this;
    }

    public override EnemyAction GetActionInfo()
    {
        return info;
    }
}

public class DragonNormalAttackState : DragonActionState
{
    string _actionName = "NormalAttack";
    
    EnemyAction _info;

    public DragonNormalAttackState(IActor enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        // �⺻���� Ÿ�̸� �ʱ�ȭ
        _dragon.ResetNormalTimer();

        _info = _dataManager.GetEnemyActionInfo(_enemy.GetName(), _actionName);
        Agent.avoidancePriority = 30;
        // ��θ� ���½����ش�
        Agent.ResetPath();
        // ���� ���� �� �÷��̾��� ��ġ�� �ٶ󺸸� ���� ��� ���
        
        PlayAnimation(_actionName);
    }

    public override void Exit()
    {
        _dragon.ResetActorList();
    }

    public override IActionState Update()
    {
        var currAnimTime = GetAnimNormalTime(_actionName);
        
        // �ִϸ��̼� ���ۺ��� 40% ������ �÷��̾ �ٶ󺸰� �ϸ鼭 �����Ѵ�.
        if (currAnimTime <= 0.3f)
        {
            _enemy.LookPlayer(false, 0.1f);
        }

        if (currAnimTime >= 0.85f)
        {
            return ChangeState(new DragonGazeState(_dragon));
        }

        return this;
    }

    public override EnemyAction GetActionInfo()
    {
        return _info;
    }
}

public class DragonDashAttackState : DragonActionState
{
    string _actionName = "DashAttack";
    EnemyAction _info;

    public DragonDashAttackState(IActor enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        // �뽬���� Ÿ�̸� �ʱ�ȭ
        _dragon.ResetDashTimer();

        _info = _dataManager.GetEnemyActionInfo(_enemy.GetName(), _actionName);

        Agent.avoidancePriority = 30;
        // ��θ� ���½����ش�
        Agent.ResetPath();
        // �뽬���� �϶��� 2��
        _animator.speed *= 1.6f;
        PlayAnimation(_actionName);
    }

    public override void Exit()
    {
        _dragon.ResetActorList();
    }

    public override IActionState Update()
    {
        var currAnimTime = GetAnimNormalTime(_actionName);

        // �ִϸ��̼� ���ۺ��� 40% ������ �÷��̾ �ٶ󺸰� �ϸ鼭 �����Ѵ�.
        if (currAnimTime <= 0.4f)
        {
            _enemy.LookPlayer(false, 0.16f);
        }

        if (currAnimTime >= 0.9f)
        {
            return ChangeState(new DragonGazeState(_dragon));
        }

        return this;
    }

    public override EnemyAction GetActionInfo()
    {
        return _info;
    }
}

public class DragonFlameAttackState : DragonActionState
{
    string _actionName = "DragonFlame";
    EnemyAction _info;

    public DragonFlameAttackState(IActor enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        _triggerTime = 0.25f;
        // �⺻���� Ÿ�̸� �ʱ�ȭ
        _dragon.ResetNormalTimer();
        _dragon.LookPlayer();
        _info = _dataManager.GetEnemyActionInfo(_enemy.GetName(), _actionName);

        Agent.avoidancePriority = 30;
        // ��θ� ���½����ش�
        Agent.ResetPath();
        PlayAnimation(_actionName);
    }

    public override void Exit()
    {
        _dragon.ResetActorList();
    }

    public override IActionState Update()
    {
        var currAnimTime = GetAnimNormalTime(_actionName);

        if(true == OnTrigger(currAnimTime))
        {
            _dragon.ExecuteFlameAttack();
            //_dragon.ExecuteMeteorAttack(2);
        }

        if (currAnimTime >= 0.99f)
        {
            return ChangeState(new DragonGazeState(_dragon));
        }

        return this;
    }

    public override EnemyAction GetActionInfo()
    {
        return _info;
    }
}

public class DragonTakeOffState : DragonActionState
{
    string _actionName = "TakeOff";

    public DragonTakeOffState(IActor enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        PlayAnimation(_actionName);
        _dragon.SetInvincible(true);
        _dragon.ExecuteDustEffect();
    }

    public override void Exit()
    {

    }

    public override IActionState Update()
    {
        var currAnimTime = GetAnimNormalTime(_actionName);

        float addScale = currAnimTime / 2f + 0.6f;

        _dragon.transform.localScale = new Vector3(addScale, addScale, addScale);

        if(currAnimTime >= 0.12f && GetPlayerDistance() <= 12f)
        {
            // �÷��̾� �ڷ� �б�
            _dragon.GetPlayer().transform.position += GetPlayerDir() * (12f - GetPlayerDistance() + 2f) * Time.deltaTime;
        }

        if (currAnimTime >= 0.99f)
        {
            return ChangeState(new DragonFlightAttackState(_dragon));
        }

        return this;
    }
}

public class DragonFlightAttackState : DragonActionState
{
    string _actionName = "DragonMeteor";
    EnemyAction _info;
    float _attackTime = 6f;
    float _totalTimer = 0f;
    float _attackTimer = 0f;
    int _attackCount = 1;
    int _count = 0;

    public DragonFlightAttackState(IActor enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        _info = _dataManager.GetEnemyActionInfo(_enemy.GetName(), _actionName);

        //if (3 <= _dragon.GetDragonCurrDiff())
        //{
        //    _attackTime *= 2f;
        //    _attackCount *= 2;
        //}

        PlayAnimation(_actionName);
    }

    public override void Exit()
    {
        
    }

    public override IActionState Update()
    {
        _totalTimer += Time.deltaTime;
        _attackTimer += Time.deltaTime;

        if (_totalTimer >= _attackTime)
        {
            return ChangeState(new DragonLandState(_dragon));
        }
        else
        {
            if (_attackTimer >= 1f)
            {
                if (_count < _attackCount)
                {
                    _count++;
                    _dragon.ExecuteMeteorAttack(_dragon.GetDragonCurrDiff());
                    _attackTimer -= 3f;
                }
            }
        }

        return this;
    }

    public override EnemyAction GetActionInfo()
    {
        return _info;
    }
}

public class DragonLandState : DragonActionState
{
    string _actionName = "Land";

    public DragonLandState(IActor enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        PlayAnimation(_actionName);
        _dragon.ExecuteDustEffect();
    }

    public override void Exit()
    {
        _dragon.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        _dragon.ResetActorList();
        _dragon.SetInvincible(false);
    }

    public override IActionState Update()
    {
        var currAnimTime = GetAnimNormalTime(_actionName);

        float addScale = (0.5f - currAnimTime / 2f) + 0.6f;

        _dragon.transform.localScale = new Vector3(addScale, addScale, addScale);

        if (currAnimTime >= 0.12f && GetPlayerDistance() <= 12f)
        {
            // �÷��̾� �ڷ� �б�
            _dragon.GetPlayer().transform.position += GetPlayerDir() * (12f - GetPlayerDistance() + 2f) * Time.deltaTime;
        }

        if (currAnimTime >= 0.99f)
        {
            return ChangeState(new DragonGazeState(_dragon));
        }

        return this;
    }
}

public class DragonDamageState : DragonActionState
{
    public DragonDamageState(IActor enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override IActionState Update()
    {
        return this;
    }
}

public class DragonScreamState : DragonActionState
{
    string _actionName = "Scream";

    public DragonScreamState(IActor enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        _dragon.SetInvincible(true);
        _triggerTime = 0.3f;
    }

    public override IActionState Update()
    {
        var currAnimTime = GetAnimNormalTime(_actionName);

        if(_dragon.GetDragonCurrDiff() >= 3)
        {
            if(true == OnTrigger(currAnimTime))
            {
                // ���� ����Ʈ ����
            }
        }

        if (currAnimTime >= 0.99f)
        {
            return new DragonGazeState(_dragon);
        }

        return this;
    }

    public override void Exit()
    {
        _dragon.SetInvincible(false);
    }
}

public class DragonDeadState : DragonActionState
{
    public DragonDeadState(IActor enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override IActionState Update()
    {
        return this;
    }
}


// �̺�Ʈ ���� ���ؼ� ���ʷ� �ѹ� ������ ����
public class DragonReadyState : DragonActionState
{
    //private InGameCamera _camera;
    private bool _isStart;
    private bool _isEnd;
    private float _timer;

    public DragonReadyState(IActor enemy) : base(enemy)
    {
        //_camera = Camera.main.GetComponent<InGameCamera>();
        Enter();
    }

    public override void Enter()
    {
        _isEnd = false;
        _isStart = false;
        _timer = 0f;
    }

    public override void Exit()
    {

    }

    public override IActionState Update()
    {
        if (GetPlayerDistance() <= 10f && false == _isStart)
        {
            // �̺�Ʈ�� ����. UI ������� �� Ŭ�����, �� ��ũ�� �� UI ���� �� ī�޶� ���� ���� �� DragonChase
            _dragon.ExecuteDragonEvent();
            _isStart = true;
        }

        return this;
    }
}

