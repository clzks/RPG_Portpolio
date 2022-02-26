using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonGazeState : EnemyActionState
{
    public DragonGazeState(BaseEnemy enemy) : base(enemy)
    {

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

public class DragonChaseState : EnemyActionState
{
    public DragonChaseState(BaseEnemy enemy) : base(enemy)
    {

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

public class DragonGroundAttackState : EnemyActionState
{
    public DragonGroundAttackState(BaseEnemy enemy) : base(enemy)
    {

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

public class DragonFlightAttackState : EnemyActionState
{
    public DragonFlightAttackState(BaseEnemy enemy) : base(enemy)
    {

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

public class DragonDamageState : EnemyActionState
{
    public DragonDamageState(BaseEnemy enemy) : base(enemy)
    {

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

public class DragonDeadState : EnemyActionState
{
    public DragonDeadState(BaseEnemy enemy) : base(enemy)
    {

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
public class DragonReadyState : EnemyActionState
{
    //private InGameCamera _camera;
    private bool _isStart;
    private bool _isEnd;
    private float _timer;

    public DragonReadyState(BaseEnemy enemy) : base(enemy)
    {
        //_camera = Camera.main.GetComponent<InGameCamera>();
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
            _enemy.ExecuteDragonEvent();
            _isStart = true;
        }

        return this;
    }
}

