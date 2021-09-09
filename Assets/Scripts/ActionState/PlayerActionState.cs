using UnityEngine;
using UnityEngine.AI;
public abstract class PlayerActionState : IActionState
{
    protected Player _player;
    protected GameManager _gameManager;
    protected NavMeshAgent Agent { get { return _player.GetNavMeshAgent(); } }
    protected MovePad _movePad { get { return _player.GetVirtualGamePad(); } }
    protected ActionPad _actionPad { get { return _player.GetActionPad(); } }
    protected Animator _animator;
    protected float _inBattleTimer = 0f;
    protected float _inNonBattleTime = 5f;
    
    public PlayerActionState(Player player)
    {
        _player = player;
        _animator = player.GetAnimator();
        _gameManager = GameManager.Get();
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

    public void PlayAnimation(string anim)
    {
        _player.PlayAnimation(anim);
    }

    public virtual ActionInfo GetActionInfo()
    {
        return null;
    }

    public DamageInfo GetDamageInfo()
    {
        return _player.GetDamageInfo();
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

    public void ResetInBattleTimer()
    {
        _inBattleTimer = 0f;
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

public abstract class PlayerAttackState : PlayerActionState
{
    protected ActionInfo info;
    protected float currAnimTime;
    protected string baseNormalAttackName = "Attack0";
    protected string actionName;
    private int _maxNormalAttackCount = 3;
    public PlayerAttackState(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        Agent.avoidancePriority = 40;
        _player.SetInBattle(true);
    }

    public override void Exit()
    {
        _player.ResetActorList();
    }

    public override ActionInfo GetActionInfo()
    {
        return info;
    }

    public override IActionState Update()
    {
        return this;
    }

    public bool IsNextAttackState()
    {
        if (currAnimTime >= info.ComboAvailableTime)
        {
            if (_player.GetCurrNormalAttackCount() < _maxNormalAttackCount)
            {
                if (true == _actionPad.GetButtonDown(out string name))
                {
                    return name == "Attack0";
                }
            }
        }
        return false;
    }
}



public class PlayerIdleState : PlayerActionState
{
    public PlayerIdleState(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        _player.ResetNormalAttackCount();
        Agent.avoidancePriority = 60;
        PlayAnimation("Idle");
    }
    public override IActionState Update()
    {
        if (true == _player.GetInBattle())
        {
            _inBattleTimer += Time.deltaTime;

            if (_inBattleTimer >= _inNonBattleTime)
            {
                _player.SetInBattle(false);
            }
        }

        if (null != GetDamageInfo())
        {
            return ChangeState(new PlayerDamageState(_player));
        }

        if(true == _actionPad.GetButtonDown(out string name))
        {
            return ChangeState(new PlayerNormalAttackState(_player));
        }

        if (true == _movePad.IsDrag())
        {
            return ChangeState(new PlayerRunState(_player));
        }

        return this;
    }

    public override void Exit()
    {

    }
}

public class PlayerRunState : PlayerActionState
{
    public PlayerRunState(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        _player.ResetNormalAttackCount();
        Agent.avoidancePriority = 60;
        PlayAnimation("Run");
    }
    public override IActionState Update()
    {
        if (true == _player.GetInBattle())
        {
            _inBattleTimer += Time.deltaTime;

            if (_inBattleTimer >= _inNonBattleTime)
            {
                _player.SetInBattle(false);
            }
        }

        if (null != GetDamageInfo())
        {
            return ChangeState(new PlayerDamageState(_player));
        }

        if (true == _actionPad.GetButtonDown(out string name))
        {
            return ChangeState(new PlayerNormalAttackState(_player));
        }

        if (true == _movePad.IsDrag())
        {
            var dir = _movePad.GetStickDirection();
            _player.SetForward(dir);
            _player.MovePlayerByPad();
        }
        else
        {
            return ChangeState(new PlayerIdleState(_player));
        }

        return this;
    }

    public override void Exit()
    {

    }
}

public class PlayerNormalAttackState : PlayerAttackState
{
    float moveDistance;
    float moveDelay;
    float moveTime;
    

    public PlayerNormalAttackState(Player player) : base(player)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        _player.AddNormalAttackCount(1);
        actionName = baseNormalAttackName + _player.GetCurrNormalAttackCount().ToString();

        if (true == _movePad.IsDrag())
        {
            var dir = _movePad.GetStickDirection();
            _player.SetForward(dir);
            _player.MovePlayerByPad();
        }

        info = _player.GetActionInfo(actionName);
        moveTime = info.MoveTime;
        
        if (null == info)
        {

        }

        PlayAnimation(actionName);
        currAnimTime = 0f;

        if (moveTime > 0f)
        {
            moveDistance = info.MoveDistance;
            moveDelay = info.MoveStartTime;
            _player.MoveCharacter(moveDelay, moveTime, moveDistance, _player.GetForward());
        }
    }

    public override IActionState Update()
    {
        currAnimTime = GetAnimNormalTime(actionName);

        if (null != GetDamageInfo())
        {
            return ChangeState(new PlayerDamageState(_player));
        }

        if(true == IsNextAttackState())
        {
            return ChangeState(new PlayerNormalAttackState(_player));
        }
        
        if(currAnimTime >= 0.99f)
        {
            if (true == _movePad.IsDrag())
            {
                return ChangeState(new PlayerRunState(_player));
            }
            else
            {
                return ChangeState(new PlayerIdleState(_player));
            }
        }

        return this;
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class PlayerDamageState : PlayerActionState
{
    float knockBackTime;
    float timer;
    float distance;
    Vector3 knockBackDir;

    public PlayerDamageState(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        _player.SetInBattle(true);
        Agent.avoidancePriority = 60;
        DamageInfo info = GetDamageInfo();
        knockBackTime = info.stiffNessTime;
        distance = info.distance;
        knockBackDir = (_player.Position - info.actorPos).normalized;
        _player.ResetDamageInfo();
        timer = 0f;
        PlayAnimation("Damage");
        _player.MoveCharacter(0.2f, distance, knockBackDir);
    }
    public override IActionState Update()
    {
        timer += Time.deltaTime;

        if(timer >= knockBackTime)
        {
            return ChangeState(new PlayerIdleState(_player));
        }

        return this;
    }

    public override void Exit()
    {
       
    }
}

public class PlayerSkillState : PlayerAttackState
{
    public PlayerSkillState(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        _player.SetInBattle(true);
    }
    public override IActionState Update()
    {
        return this;
    }

    public override void Exit()
    {

    }
}

public class PlayerDieState : PlayerActionState
{
    public PlayerDieState(Player player) : base(player)
    {

    }

    public override void Enter()
    {
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

