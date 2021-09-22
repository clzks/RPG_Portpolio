using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseBuff : IBuff
{
    private int _id;
    private float _life;
    private float _tick;

    public virtual void TakeActor(IActor actor)
    {
        var info = DataManager.Get().GetBuffInfo(_id);
        var status = actor.GetValidStatus();


        switch (info.Type)
        {
            case BuffType.Damage:
                status.Damage += info.Value;
                break;

            case BuffType.AttackSpeed:
                status.AttackSpeed += info.Value;
                break;

            case BuffType.Speed:
                status.Speed += info.Value;
                break;

            case BuffType.Dot:
                status.CurrHp -= info.Value;
                break;

            case BuffType.Count:

                break;
        }
    }

    public virtual void Update(float tick, IActor actor)
    {
        _life -= tick;

        if(_life <= 0f)
        {
            actor.RemoveBuff(this);
        }
        else
        {
            TakeActor(actor);
        }
    }
}

