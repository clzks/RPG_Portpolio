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
    protected string actionName;
    public PlayerActionState(Player player, string action)
    {
        _player = player;
        _animator = player.GetAnimator();
        _gameManager = GameManager.Get();
        actionName = action;
        Enter();
    }

    public abstract void Enter();
    public abstract IActionState Update();
    public abstract void Exit();

    protected IActionState ChangeState(IActionState state)
    {
        Exit();
        return state;
    }

    protected void PlayAnimation(string anim)
    {
        _player.PlayAnimation(anim);
    }

    protected void PlayAnimation()
    {
        _player.PlayAnimation(actionName);
    }


    public virtual ActionInfo GetActionInfo()
    {
        return null;
    }

    protected DamageInfo GetDamageInfo()
    {
        return _player.GetDamageInfo();
    }

    protected float GetAnimNormalTime(string anim)
    {
        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float currAnimTime = 0f;
        if (stateInfo.IsName(anim))
        {
            currAnimTime = stateInfo.normalizedTime;
        }

        return currAnimTime;
    }

    protected float GetAnimTotalTime(string anim)
    {
        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float currAnimTime = 0f;
        if (stateInfo.IsName(anim))
        {
            currAnimTime = stateInfo.length / stateInfo.speed;
        }

        return currAnimTime;
    }

    protected void ResetInBattleTimer()
    {
        _inBattleTimer = 0f;
    }

    protected void SetAvoidancePriority(int value)
    {
        if(null != Agent)
        {
            Agent.avoidancePriority = value;
        }
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
    //protected string baseNormalAttackName = "Attack0";
    
    private int _maxNormalAttackCount = 3;
    public PlayerAttackState(Player player, string action) : base(player, action)
    {

    }

    public override void Enter()
    {
        SetAvoidancePriority(40);
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

    public bool IsNextAttackState(out string actionName)
    {
        actionName = string.Empty;

        if (currAnimTime >= info.ComboAvailableTime)
        {
            if (_player.GetCurrNormalAttackCount() < _maxNormalAttackCount)
            {
                bool takeNextState = _actionPad.GetButtonDown(out string name);
                actionName = name;
                return takeNextState;
            }
        }

        return false;
    }
}



public class PlayerIdleState : PlayerActionState
{
    public PlayerIdleState(Player player, string action = "Idle") : base(player, action)
    {
        
    }

    public override void Enter()
    {
        _player.ResetNormalAttackCount();
        SetAvoidancePriority(60);
        PlayAnimation();
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
            if (string.Equals(name, "Attack0"))
            {
                return ChangeState(new PlayerNormalAttackState(_player));
            }
            else
            {
                return ChangeState(new PlayerSkillState(_player, name));
            }
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
    public PlayerRunState(Player player , string action = "Run") : base(player, action)
    {
        
    }

    public override void Enter()
    {
        _player.ResetNormalAttackCount();
        SetAvoidancePriority(60);
        PlayAnimation();
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
            if (string.Equals(name, "Attack0"))
            {
                return ChangeState(new PlayerNormalAttackState(_player));
            }
            else
            {
                return ChangeState(new PlayerSkillState(_player, name));
            }
        }

        if (true == _movePad.IsDrag())
        {
            var dir = _movePad.GetStickDirection();
            _player.SetForward(dir);
            _player.MovePlayerByPad();
        }
        else
        {
            return ChangeState(new PlayerIdleState(_player, "Idle"));
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
    

    public PlayerNormalAttackState(Player player, string action = "Attack0") : base(player, action)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        _player.AddNormalAttackCount(1);
        actionName += _player.GetCurrNormalAttackCount().ToString();

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

        PlayAnimation();
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

        if(true == IsNextAttackState(out string name))
        {
            if (string.Equals(name, "Attack0"))
            {
                return ChangeState(new PlayerNormalAttackState(_player));
            }
            else
            {
                return ChangeState(new PlayerSkillState(_player, name));
            }
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

    public PlayerDamageState(Player player, string action = "Damage") : base(player, action)
    {

    }

    public override void Enter()
    {
        _player.SetInBattle(true);
        SetAvoidancePriority(60);
        DamageInfo info = GetDamageInfo();
        knockBackTime = info.stiffNessTime;
        distance = info.distance;
        knockBackDir = (_player.Position - info.actorPos).normalized;
        _player.ResetDamageInfo();
        timer = 0f;
        PlayAnimation();
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
    public PlayerSkillState(Player player, string action) : base(player, action)
    {

    }

    public override void Enter()
    {
        _player.SetInBattle(true);
    }

    public override IActionState Update()
    {
        if (null == _player.GetActionInfo(actionName))
        {
            return new PlayerIdleState(_player, "Idle");
        }

        return this;
    }

    public override void Exit()
    {

    }
}

public class PlayerDieState : PlayerActionState
{
    public PlayerDieState(Player player, string action = "Die") : base(player, action)
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

