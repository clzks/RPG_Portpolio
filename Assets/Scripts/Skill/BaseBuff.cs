using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseBuff : IBuff
{
    protected int _id;
    protected string _name;
    protected float _life;
    protected float _lifeValue;
    protected float _value;
    protected BaseEffect _effect;
    private float _tick;
    protected BuffIcon _icon;

    public BaseBuff(BuffInfo info)
    {
        _id = info.Id;
        _name = info.Name;
        _lifeValue = info.Life;
        _life = _lifeValue;
        _tick = info.Tick;
        _value = info.Value;
    }

    public virtual void SetEffect(BaseEffect effect)
    {
        _effect = effect;
    }

    //public virtual void StartBuff(IActor actor)
    //{
    //    if (true == actor.AddBuff(this))
    //    {
    //        SetActiveEffect(actor, true);
    //    }
    //}

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
                actor.GetOriginStatus().Shield = _value;
                break;

            case BuffType.Count:

                break;
        }
    }

    public virtual void Update(float tick, IActor actor)
    {
        _life -= tick;

        if(null != _icon)
        {
            _icon.SetFillAmount(_life / _lifeValue);
        }

        if(_life <= 0f)
        {
            ResetBuff(actor);
        }
        else
        {
            TakeActor(actor);
        }
    }

    public virtual void SetBuffIcon(BuffIcon buffIcon, Sprite sprite)
    {
        _icon = buffIcon;
        _icon.SetIconImage(sprite);
    }

    public int GetId()
    {
        return _id;
    }
    
    public string GetName()
    {
        return _name;
    }

    public virtual void Renew(IActor actor = null)
    {
        if (_life <= _lifeValue)
        {
            _life = _lifeValue;
        }
    }

    public virtual void ResetBuff(IActor actor)
    {
        if(null != _icon)
        {
            _icon.ReturnObject();
        }
        actor.RemoveBuff(this);
        _effect.ReturnObject();
        actor.RemoveBuff(this);
    }
}

