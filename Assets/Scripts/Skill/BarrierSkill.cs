
using UnityEngine;

public class BarrierSkill : BaseBuff
{
    public BarrierSkill(BuffInfo info) : base(info)
    {

    }

    public override void TakeActor(IActor actor)
    {
        // actor에 이펙트 붙여주는 코드
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
