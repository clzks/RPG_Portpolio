using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseBuff : IBuff
{
    protected int _id;
    protected float _life;
    private float _lifeValue;
    protected float _value;
    protected BaseEffect _effect;
    private float _tick;
    private float _timer;

    public BaseBuff(BuffInfo info, BaseEffect effect = null)
    {
        _id = info.Id;
        _lifeValue = info.Life;
        _life = _lifeValue;
        _tick = info.Tick;
        _value = info.Value;
        _timer = 0;
        _effect = effect;
    }

    public virtual void StartBuff(IActor actor)
    {
        if (true == actor.AddBuff(this))
        {
            SetActiveEffect(actor, true);
        }
    }

    public virtual void TakeActor(IActor actor)
    {
        var info = DataManager.Get().GetBuffInfo(_id);
        var status = actor.GetValidStatus();

        switch (info.Type)
        {
            case BuffType.Attack:
                status.Attack += _value;
                break;

            case BuffType.AttackSpeed:
                status.AttackSpeed += _value;
                break;

            case BuffType.Speed:
                status.Speed += _value;
                break;

            case BuffType.Dot:
                status.CurrHp -= _value;
                break;

            case BuffType.Shield:
                status.Shield += _value;
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
            SetActiveEffect(actor, false);
            actor.RemoveBuff(this);
        }
        else
        {
            TakeActor(actor);
        }
    }

    public virtual void SetActiveEffect(IActor actor, bool enabled)
    {

    }

    public int GetId()
    {
        return _id;
    }

    public void Renew()
    {
        _life = _lifeValue;
    }

    public virtual void ResetBuff(IActor actor)
    {
        SetActiveEffect(actor, false);
        actor.RemoveBuff(this);
        _effect.ReturnObject();
    }
}

