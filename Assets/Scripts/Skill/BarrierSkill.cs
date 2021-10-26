using UnityEngine;

public class BarrierSkill : BaseBuff
{
    public BarrierSkill(BuffInfo info, BaseEffect effect = null) : base(info, effect)
    {

    }

    public override void StartBuff(IActor actor)
    {
        TakeActor(actor);

        base.StartBuff(actor);
    }

    public override void TakeActor(IActor actor)
    {
        // actor�� ����Ʈ �ٿ��ִ� �ڵ�
        var status = actor.GetOriginStatus();
        status.Shield += _value;
    }

    public override void Update(float tick, IActor actor)
    {
        _life -= tick;

        if(actor.GetShield() <= 0f || _life <= 0f)
        {
            ResetBuff(actor);
        }
    }

    public override void ResetBuff(IActor actor)
    {
        actor.ResetShield();
        base.ResetBuff(actor);
    }
}
