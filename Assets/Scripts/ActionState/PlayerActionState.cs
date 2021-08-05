using UnityEngine;

public abstract class PlayerActionState : IActionState
{
    protected Player _player;
    protected GameManager _gameManager;
    protected VirtualGamePad _movePad;
    protected Animator _animator;
    //protected bool isPossibleCombo;
    //protected bool isActionEnd;

    public PlayerActionState(Player player)
    {
        _player = player;
        _movePad = player.movePad;
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

    
public class PlayerIdleState : PlayerActionState
{
    public PlayerIdleState(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        PlayAnimation("Idle");
    }
    public override IActionState Update()
    {
        if(true == _player.attackButton.GetButtonDown())
        {
            return ChangeState(new PlayerAttackOneState(_player));
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
        PlayAnimation("Run");
    }
    public override IActionState Update()
    {
        if (true == _player.attackButton.GetButtonDown())
        {
            return ChangeState(new PlayerAttackOneState(_player));
        }

        if (true == _movePad.IsDrag())
        {
            var dir = _movePad.GetStickDirection();
            _player.SetFoward(dir);
            _player.MovePlayer();
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

public class PlayerAttackOneState : PlayerActionState
{
    ActionInfo info;
    string actionName = "Attack01";
    
    public PlayerAttackOneState(Player player) : base(player)
    {
        info = player.GetActionInfo(actionName);
        
        if(null == info)
        {
            
        }
    }

    public override void Enter()
    {
        PlayAnimation(actionName);
    }

    public override IActionState Update()
    {
        var currAnimTime = GetAnimNormalTime(actionName);

        if (currAnimTime >= info.ComboAvailableTime)
        {
            if (true == _player.attackButton.GetButtonDown())
            {
                return ChangeState(new PlayerAttackTwoState(_player));
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
        _player.ResetActorList();
    }

    public override ActionInfo GetActionInfo()
    {
        return info;
    }
}

public class PlayerAttackTwoState : PlayerActionState
{
    ActionInfo info;
    string actionName = "Attack02";
    public PlayerAttackTwoState(Player player) : base(player)
    {
        info = player.GetActionInfo(actionName);

        if (null == info)
        {

        }
    }

    public override void Enter()
    {
        PlayAnimation(actionName);
    }

    public override IActionState Update()
    {
        var currAnimTime = GetAnimNormalTime(actionName);

        if (currAnimTime >= info.ComboAvailableTime)
        {
            if (true == _player.attackButton.GetButtonDown())
            {
                return ChangeState(new PlayerAttackThreeState(_player));
            }
        }

        if (currAnimTime >= 0.99f)
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
        _player.ResetActorList();
    }

    public override ActionInfo GetActionInfo()
    {
        return info;
    }
}

public class PlayerAttackThreeState : PlayerActionState
{
    ActionInfo info;
    string actionName = "Attack03";
    public PlayerAttackThreeState(Player player) : base(player)
    {
        info = player.GetActionInfo(actionName);

        if (null == info)
        {

        }
    }

    public override void Enter()
    {
        PlayAnimation(actionName);
    }

    public override IActionState Update()
    {
        var currAnimTime = GetAnimNormalTime(actionName);

        if (currAnimTime >= info.ComboAvailableTime)
        {

        }

        if (currAnimTime >= 0.99f)
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
        _player.ResetActorList();
    }

    public override ActionInfo GetActionInfo()
    {
        return info;
    }
}

public class PlayerDamageState : PlayerActionState
{
    public PlayerDamageState(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        PlayAnimation("Damage");
    }
    public override IActionState Update()
    {
        return this;
    }

    public override void Exit()
    {

    }
}

public class PlayerSkillState : PlayerActionState
{
    public PlayerSkillState(Player player) : base(player)
    {
    }

    public override void Enter()
    {

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

