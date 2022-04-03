using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class DragonActionState : EnemyActionState
{
    protected Dragon _dragon;

    protected float minNormalAttackRange = 3.5f;
    protected float maxNormalAttackRange = 5.5f;

    protected float minDashAttackRange = 5.5f;
    protected float maxDashAttackRange = 8.0f;

    protected float maxFlameRange = 12.0f;

    protected bool _isTriggerOn = false;
    protected float _triggerTime = -1f;
    
    protected float _animSpeed = 1f;

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

    protected void ResetDashTimer()
    {
        _dragon.ResetDashTimer();
    }

    protected void ResetMeteorTimer()
    {
        _dragon.ResetMeteorTimer();
    }

    protected void ResetFlameTimer()
    {
        _dragon.ResetFlameTimer();
    }

    protected void UpdateAttackTimer()
    {
        _dragon.UpdateAttackTimer();
    }

    protected void SetDragonSpeed()
    {
        _dragon.GetNavMeshAgent().angularSpeed = 120f;

        int currDiff = _dragon.GetDragonCurrDiff();

        switch (currDiff)
        {
            case 1:
                _dragon.GetNavMeshAgent().speed = 6f;
                break;

            case 2:
                _dragon.GetNavMeshAgent().speed = 8f;
                break;

            case 3:
                _dragon.GetNavMeshAgent().speed = 10f;
                break;
        }

        _animator.speed = 1f;
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

        float changeTime = 0f;
        int difficulty = _dragon.GetDragonCurrDiff();

        switch (difficulty)
        {
            case 1:
                changeTime = 1f;
                break;

            case 2:
                changeTime = 1f;
                break;

            case 3:
                changeTime = 0f;
                break;

            default:

                break;
        }

        if (_timer <= changeTime)
        {
            return this;
        }

        UpdateAttackTimer();

        float distance = GetPlayerDistance();
        float baseDistance = GetBaseDistance();

        if(true == CheckDifficultyChance())
        {
            _dragon.ChangeDifficulty();
            return ChangeState(new DragonScreamState(_dragon));
        }

        // 베이스에서 20만큼 떨어진 상태에서 플레이어와 거리가 5만큼 차이나면 귀환
        // 해당 부분은 추후 폐쇄된 맵으로 변경 시 삭제될 가능성 있음
        if (distance >= 5f && baseDistance >= 20f)
        {
            return ChangeState(new DragonReturnState(_dragon));
        }

        return ChangeState(new DragonChaseState(_dragon));
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
        if (_dragon.GetDragonCurrDiff() >= 3)
        {
            PlayAnimation("Run");
        }
        else
        {
            PlayAnimation("Walk");
        }

        int difficulty = _dragon.GetDragonCurrDiff();

        switch (difficulty)
        {
            case 1:
                _animator.speed = 1f;
                break;

            case 2:
                _animator.speed = 1.2f;
                break;

            case 3:
                _animator.speed = 1.4f;
                break;
        }
    }

    public override void Exit()
    {

    }

    public override IActionState Update()
    {
        float distance = GetPlayerDistance();

        UpdateAttackTimer();

        if (true == CheckDifficultyChance())
        {
            _dragon.ChangeDifficulty();
            return ChangeState(new DragonScreamState(_dragon));
        }

        int difficulty = _dragon.GetDragonCurrDiff();

        // 메테오 쿨타임이 돌았을 경우 메테오 패턴이 1순위
        if (_dragon.GetMeteorTimer() <= 0f)
        {
            return ChangeState(new DragonTakeOffState(_dragon));
        }

        switch (difficulty)
        {
            case 1:
                // 용의 시야 좌우 30도 내에 플레이어가 위치했을 경우
                if (true == Formula.IsTargetInSight(_dragon.transform.forward, 30f, _dragon.GetPosition(), _dragon.GetPlayer().Position))
                {
                    SetDragonSpeed();

                    // 매우 가까이 있을 경우
                    if (distance < minNormalAttackRange)
                    {
                        return ChangeState(new DragonNormalAttackState(_dragon));
                    }
                    // 일반공격 사거리 내에 위치할 때    
                    else if (distance >= minNormalAttackRange && distance <= maxNormalAttackRange)
                    {
                        return ChangeState(new DragonNormalAttackState(_dragon));
                    }

                }
                // 시야 내에 없을 경우
                else
                {
                    _dragon.LookPlayer(false, 0.016f);
                }
                break;

            case 2:
                // 용의 시야 좌우 30도 내에 플레이어가 위치했을 경우
                if (true == Formula.IsTargetInSight(_dragon.transform.forward, 30f, _dragon.GetPosition(), _dragon.GetPlayer().Position))
                {
                    SetDragonSpeed();

                    // 매우 가까이 있을 경우
                    if (distance < minNormalAttackRange)
                    {
                        return ChangeState(new DragonNormalAttackState(_dragon));
                    }
                    // 일반공격 사거리 내에 위치할 때    
                    else if (distance >= minNormalAttackRange && distance <= maxNormalAttackRange)
                    {
                        return ChangeState(new DragonNormalAttackState(_dragon));
                    }
                    // 더 멀리 있을 경우
                    else
                    {
                        // 대쉬 공격 쿨타임 돌았을 경우
                        if (_dragon.GetDashTimer() <= 0f)
                        {
                            if (distance >= minDashAttackRange && distance <= maxDashAttackRange)
                            {
                                return ChangeState(new DragonDashAttackState(_dragon));
                            }
                        }
                        else
                        {
                            if(_dragon.GetFlameTimer() <= 0f)
                            {
                                if(distance <= maxFlameRange)
                                {
                                    return ChangeState(new DragonFlameAttackState(_dragon));
                                }
                            }
                        }
                    }
                }
                // 시야 내에 없을 경우
                else
                {
                    _dragon.LookPlayer(false, 0.016f);
                }
                break;

            case 3:
                if (true == Formula.IsTargetInSight(_dragon.transform.forward, 30f, _dragon.GetPosition(), _dragon.GetPlayer().Position))
                {
                    SetDragonSpeed();

                    // 매우 가까이 있을 경우
                    if (distance < minNormalAttackRange)
                    {
                        return ChangeState(new DragonNormalAttackState(_dragon));
                    }
                    // 일반공격 사거리 내에 위치할 때    
                    else if (distance >= minNormalAttackRange && distance <= maxNormalAttackRange)
                    {
                        if (_dragon.GetBurstTimer() <= 0f)
                        {
                            return ChangeState(new DragonBurstAttackState(_dragon));
                        }
                        else
                        {
                            return ChangeState(new DragonNormalAttackState(_dragon));
                        }
                    }
                    // 더 멀리 있을 경우
                    else
                    {
                        // 대쉬 공격 쿨타임 돌았을 경우
                        if (_dragon.GetDashTimer() <= 0f)
                        {
                            if (distance <= maxDashAttackRange)
                            {
                                return ChangeState(new DragonDashAttackState(_dragon));
                            }
                        }
                        else
                        {
                            if (_dragon.GetFlameTimer() <= 0f)
                            {
                                if (distance <= maxFlameRange)
                                {
                                    return ChangeState(new DragonFlameAttackState(_dragon));
                                }
                            }
                        }
                    }
                }
                // 시야 내에 없을 경우
                else
                {
                    if(distance <= maxNormalAttackRange && _dragon.GetBurstTimer() <= 0f)
                    {
                        return ChangeState(new DragonBurstAttackState(_dragon));
                    }

                    _dragon.LookPlayer(false, 0.016f);
                }
                break;
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

public class DragonReturnState : DragonActionState
{
    public DragonReturnState(IActor enemy) : base(enemy)
    {
        Enter();
    }


    public override void Enter()
    {
        if (_dragon.GetDragonCurrDiff() >= 3)
        {
            PlayAnimation("Run");
        }
        else
        {
            PlayAnimation("Walk");
        }

        Agent.SetDestination(_dragon.GetBaseCamp().position);
    }

    public override void Exit()
    {
        
    }

    public override IActionState Update()
    {
        if(true == IsArriveToDest())
        {
            return ChangeState(new DragonGazeState(_dragon));
        }

        float distance = GetPlayerDistance();

        if(distance <= 5f)
        {
            return ChangeState(new DragonChaseState(_dragon));
        }

        return this;
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

        info = _dataManager.GetEnemyActionInfo(_enemy.GetName(), _actionName);
        Agent.avoidancePriority = 30;
        // 경로를 리셋시켜준다
        Agent.ResetPath();
        // 공격 시작 시 플레이어의 위치를 바라보며 공격 모션 재생
        //_enemy.LookPlayer();
        _animator.speed *= 1.5f;
        PlayAnimation(_actionName);
    }

    public override void Exit() 
    {
        _dragon.ResetBurstTimer();
        _dragon.ResetActorList();
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
        _info = _dataManager.GetEnemyActionInfo(_enemy.GetName(), _actionName);
        Agent.avoidancePriority = 30;
        // 경로를 리셋시켜준다
        Agent.ResetPath();
        // 공격 시작 시 플레이어의 위치를 바라보며 공격 모션 재생
        
        PlayAnimation(_actionName);
    }

    public override void Exit()
    {
        _dragon.ResetActorList();
    }

    public override IActionState Update()
    {
        var currAnimTime = GetAnimNormalTime(_actionName);
        
        // 애니메이션 시작부터 40% 까지는 플레이어를 바라보게 하면서 공격한다.
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
    float _dashAnimSpeed = 1.6f;

    public DragonDashAttackState(IActor enemy) : base(enemy)
    {
        Enter();
    }

    public override void Enter()
    {
        // 대쉬공격 타이머 초기화
        _dragon.ResetDashTimer();

        _info = _dataManager.GetEnemyActionInfo(_enemy.GetName(), _actionName);

        Agent.avoidancePriority = 30;
        // 경로를 리셋시켜준다
        Agent.ResetPath();
        // 대쉬공격 일때만 2배
        _animator.speed *= 1.5f;
        PlayAnimation(_actionName);
    }

    public override void Exit()
    {
        _dragon.ResetActorList();
    }

    public override IActionState Update()
    {
        var currAnimTime = GetAnimNormalTime(_actionName);

        // 애니메이션 시작부터 40% 까지는 플레이어를 바라보게 하면서 공격한다.
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
        _dragon.LookPlayer();
        _info = _dataManager.GetEnemyActionInfo(_enemy.GetName(), _actionName);

        Agent.avoidancePriority = 30;
        // 경로를 리셋시켜준다
        Agent.ResetPath();
        PlayAnimation(_actionName);
    }

    public override void Exit()
    {
        _dragon.AddMeteorTime(2f);
        _dragon.ResetFlameTimer();
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

        float addScale = currAnimTime / 3.33f + 0.6f;

        _dragon.transform.localScale = new Vector3(addScale, addScale, addScale);

        if(currAnimTime >= 0.12f && GetPlayerDistance() <= 12f)
        {
            // 플레이어 뒤로 밀기
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
    int _attackCount = 2;
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
                    _attackTimer -= 2f;
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
        _dragon.ResetMeteorTimer();
    }

    public override IActionState Update()
    {
        var currAnimTime = GetAnimNormalTime(_actionName);

        float addScale = (0.3f - currAnimTime / 3.33f) + 0.6f;

        _dragon.transform.localScale = new Vector3(addScale, addScale, addScale);

        if (currAnimTime >= 0.12f && GetPlayerDistance() <= 12f)
        {
            // 플레이어 뒤로 밀기
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
        Agent.ResetPath();
        PlayAnimation(_actionName);
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
                // 광폭 이펙트 생성
                _dragon.ExecuteFrenzyEffect();
            }
        }

        if (currAnimTime >= 0.99f)
        {
            return ChangeState(new DragonGazeState(_dragon));
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


// 이벤트 씬을 위해서 최초로 한번 실행할 상태
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
            // 이벤트씬 시작. UI 사라지고 용 클로즈업, 용 스크림 후 UI 복구 및 카메라 각도 복구 후 DragonChase
            _dragon.ExecuteDragonEvent();
            _isStart = true;
        }

        return this;
    }
}

