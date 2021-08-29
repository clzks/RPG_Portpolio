using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    public float CurrHp;
    public float MaxHp;
    public float Speed;
    public float AtkSpeed;
    public float Stamina;
    public float Strength;
    public bool IsInvincible;

    public static Status MakeSampleStatus()
    {
        Status st = new Status();
        st.MaxHp = 100f;
        st.CurrHp = st.MaxHp;
        st.Speed = 5f;
        st.AtkSpeed = 1f;
        st.Stamina = 100f;
        st.Strength = 1f;
        st.IsInvincible = false;
        return st;
    }
}
