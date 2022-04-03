using UnityEngine;

public class BarrierSkill : BaseBuff
{
   
    public BarrierSkill(BuffInfo info) : base(info)
    {

    }

    public override void TakeActor(IActor actor)
    {
        base.TakeActor(actor);
    }

    public override void Renew(IActor actor)
    {
        base.TakeActor(actor);
        base.Renew();
    }

    public override void SetBuffIcon(BuffIcon buffIcon, Sprite sprite)
    {
        base.SetBuffIcon(buffIcon, sprite);
    }

    public override void Update(float tick, IActor actor)
    {
        _life -= tick;

        if (null != _icon)
        {
            _icon.SetFillAmount(_life / _lifeValue);
        }

        if (actor.GetShield() <= 0f || _life <= 0f)
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
