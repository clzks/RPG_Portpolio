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
        var a = _animator.runtimeAnimatorController.animationClips;
        AnimationClip c;
        foreach (var item in a)
        {
            
        }

        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float currAnimTime = 0f;
        if (stateInfo.IsName(anim))
        {
            currAnimTime = stateInfo.length / stateInfo.speed;
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

public abstract class PlayerNormalAttackState : PlayerActionState
{
    protected ActionInfo info;
    protected float currAnimTime;
    public PlayerNormalAttackState(Player player) : base(player)
    {

    }

    public override void Enter()
    {
       
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
            if (true == _player.attackButton.GetButtonDown())
            {
                return true;
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
        PlayAnimation("Idle");
        Debug.Log(GetAnimTotalTime("Idle"));
    }
    public override IActionState Update()
    {
        if(null != GetDamageInfo())
        {
            return ChangeState(new PlayerDamageState(_player));
        }

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
        if (null != GetDamageInfo())
        {
            return ChangeState(new PlayerDamageState(_player));
        }

        if (true == _player.attackButton.GetButtonDown())
        {
            return ChangeState(new PlayerAttackOneState(_player));
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

public class PlayerAttackOneState : PlayerNormalAttackState
{
    string actionName = "Attack01";
    float moveDistance;
    float moveDelay;
    float moveTime;

    public PlayerAttackOneState(Player player) : base(player)
    {
        
    }

    public override void Enter()
    {
        if (true == _movePad.IsDrag())
        {
            var dir = _movePad.GetStickDirection();
            _player.SetForward(dir);
            _player.MovePlayerByPad();
        }

        info = _player.GetActionInfo(actionName);
        moveDistance = info.MoveDistance;
        moveDelay = info.MoveStartTime;
        moveTime = info.MoveTime;

        if (null == info)
        {

        }

        PlayAnimation(actionName);
        Debug.Log(GetAnimTotalTime(actionName));
        currAnimTime = 0f;
        _player.MoveCharacter(moveDelay, moveTime, moveDistance, _player.GetForward());
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
            return ChangeState(new PlayerAttackTwoState(_player));
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

public class PlayerAttackTwoState : PlayerNormalAttackState
{
    string actionName = "Attack02";
    float moveDistance;
    float moveDelay;
    float moveTime;
    public PlayerAttackTwoState(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        if (true == _movePad.IsDrag())
        {
            var dir = _movePad.GetStickDirection();
            _player.SetForward(dir);
            _player.MovePlayerByPad();
        }

        info = _player.GetActionInfo(actionName);
        moveDistance = info.MoveDistance;
        moveDelay = info.MoveStartTime;
        moveTime = info.MoveTime;

        if (null == info)
        {

        }

        PlayAnimation(actionName);
        currAnimTime = 0f;
        _player.MoveCharacter(moveDelay, moveTime, moveDistance, _player.GetForward());
    }

    public override IActionState Update()
    {
        currAnimTime = GetAnimNormalTime(actionName);

        if (null != GetDamageInfo())
        {
            return ChangeState(new PlayerDamageState(_player));
        }

        if (true == IsNextAttackState())
        {
            return ChangeState(new PlayerAttackThreeState(_player));
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
        base.Exit();
    }
}

public class PlayerAttackThreeState : PlayerNormalAttackState
{
    string actionName = "Attack03";
    float moveDistance;
    float moveDelay;
    float moveTime;

    public PlayerAttackThreeState(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        if (true == _movePad.IsDrag())
        {
            var dir = _movePad.GetStickDirection();
            _player.SetForward(dir);
            _player.MovePlayerByPad();
        }

        info = _player.GetActionInfo(actionName);
        moveDistance = info.MoveDistance;
        moveDelay = info.MoveStartTime;
        moveTime = info.MoveTime;

        if (null == info)
        {

        }

        PlayAnimation(actionName);
        currAnimTime = 0f;
        _player.MoveCharacter(moveDelay, moveTime, moveDistance, _player.GetForward());
    }

    public override IActionState Update()
    {
        currAnimTime = GetAnimNormalTime(actionName);

        if (null != GetDamageInfo())
        {
            return ChangeState(new PlayerDamageState(_player));
        }


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
        DamageInfo info = GetDamageInfo();
        knockBackTime = info.stiffNessTime;
        distance = info.distance;
        knockBackDir = (_player.Position - info.actorPos).normalized;
        timer = 0f;
        PlayAnimation("Damage");
        //_player.MoveCharacter(GetAnimTotalTime("Damage"), distance, knockBackDir);
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
        _player.ResetDamageInfo();
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

