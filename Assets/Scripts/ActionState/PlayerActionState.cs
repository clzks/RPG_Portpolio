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
        //Enter();
    }

    public abstract void Enter();
    public abstract IActionState Update();
    public abstract void Exit();

    public IActionState ChangeState(IActionState state)
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

    protected void PlayAnimation(float startTime)
    {
        _player.PlayAnimation(actionName, startTime);
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
    
    private int _maxNormalAttackCount = 3;
    public PlayerAttackState(Player player, string action) : base(player, action)
    {
        //Enter();
    }

    public override void Enter()
    {
        currAnimTime = 0;
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
                var actionButton = _actionPad.GetClickedButton();

                if (null != actionButton)
                {
                    actionName = actionButton.GetActionName();
                    actionButton.ExecuteButton();
                    return true;
                }
            }
        }

        return false;
    }
}



public class PlayerIdleState : PlayerActionState
{
    public PlayerIdleState(Player player, string action = "Idle") : base(player, action)
    {
        Enter();
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

        var actionButton = _actionPad.GetClickedButton();

        if (null != _actionPad.GetClickedButton())
        {
            string name = actionButton.GetActionName();

            if (string.Equals(name, "Attack0"))
            {
                return ChangeState(new PlayerNormalAttackState(_player));
            }
            else if (string.Equals(name, "Roll"))
            {
                return ChangeState(new PlayerRollState(_player));
            }
            else
            {
                actionButton.ExecuteButton();
                return ChangeState(new PlayerSkillState(_player, name));
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            return ChangeState(new PlayerRollState(_player));
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
        Enter();
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

        var actionButton = _actionPad.GetClickedButton();
      
        if (null != actionButton)
        {
            string name = actionButton.GetActionName();

            if (string.Equals(name, "Attack0"))
            {
                return ChangeState(new PlayerNormalAttackState(_player));
            }
            else if(string.Equals(name, "Roll"))
            {
                return ChangeState(new PlayerRollState(_player));
            }
            else
            {
                actionButton.ExecuteButton();
                return ChangeState(new PlayerSkillState(_player, name));
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            return ChangeState(new PlayerRollState(_player));
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
        Enter();
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
        
        if (null == info)
        {

        }

        currAnimTime = 0f;
        PlayAnimation();

        moveTime = info.MoveTime;
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
            else if(string.Equals(name, "Roll"))
            {
                return ChangeState(new PlayerRollState(_player));
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
    bool isStun = false;

    public PlayerDamageState(Player player, string action = "Damage") : base(player, action)
    {
        Enter();
    }

    public override void Enter()
    {
        // 피격시 공격버튼 초기화 시켜줌
        _actionPad.GetActionButton(0).ExecuteButton();

        _player.ResetNormalAttackCount();
        _player.SetInBattle(true);
        SetAvoidancePriority(60);
        DamageInfo info = GetDamageInfo();
        if(info.distance > 1.5f)
        {
            isStun = true;
        }
        knockBackTime = info.stiffNessTime;
        distance = info.distance;
        knockBackDir = (_player.Position - info.actorPos).normalized;
        // ResetDamageInfo를 Exit으로 빼면 피격 모션동안 무적 상태 가능할듯
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
            if (false == isStun)
            {
                return ChangeState(new PlayerIdleState(_player));
            }
            else
            {
                return ChangeState(new PlayerStunState(_player));
            }
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
        Enter();
    }

    public override void Enter()
    {
        base.Enter();
        info = _player.GetActionInfo(actionName);
        if(info.AnimationStartTime > 0f)
        {
            PlayAnimation(info.AnimationStartTime);
        }
        else
        {
            PlayAnimation();
        }
        _player.SetInvincible(true);
    }

    public override IActionState Update()
    {
        currAnimTime = GetAnimNormalTime(actionName);

        if (null == _player.GetActionInfo(actionName))
        {
            return ChangeState(new PlayerIdleState(_player));
        }

        if (currAnimTime >= info.AnimationEndTime)
        {
            return ChangeState(new PlayerIdleState(_player));
        }

        return this;
    }

    public override void Exit()
    {
        _player.SetInvincible(false);
    }
}

public class PlayerRollState : PlayerActionState
{
    float timer;
    float speed;
    float currAnimTime;

    public PlayerRollState(Player player, string action = "Roll") : base(player, action)
    {
        Enter();
    }

    public override void Enter()
    {
        timer = 0f;
        speed = 6f;
        currAnimTime = 0f;

        _player.ResetNormalAttackCount();
        PlayAnimation(actionName);

        _player.SetInvincible(true);
        _player.MoveCharacter(0.05f, 0.25f, 3.5f, _player.transform.forward);
    }

    public override IActionState Update()
    {
        currAnimTime = GetAnimNormalTime(actionName);
        
        if (currAnimTime >= 0.99f)
        {
            return ChangeState(new PlayerIdleState(_player));
        }

        return this;
    }

    public override void Exit()
    {
        _player.SetInvincible(false);
    }
}

public class PlayerStunState : PlayerActionState
{
    float stunTime = 2f;
    float timer = 0f;

    public PlayerStunState(Player player, string action = "Dizzy") : base(player, action)
    {
        Enter();
    }

    public override void Enter()
    {
        _player.ResetNormalAttackCount();
        PlayAnimation(actionName);
    }
    public override IActionState Update()
    {
        timer += Time.deltaTime;

        if(timer >= stunTime)
        {
            return ChangeState(new PlayerIdleState(_player));
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
        Enter();
    }

    public override void Enter()
    {
        _player.ResetNormalAttackCount();
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

