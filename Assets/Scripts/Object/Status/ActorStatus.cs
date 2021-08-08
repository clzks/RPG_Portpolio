using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    public float Hp;
    public float Speed;
    public float AtkSpeed;
    public float Stamina;
    public float Strength;
    public bool IsInvincible;

    public static Status MakeSampleStatus()
    {
        Status st = new Status();
        st.Hp = 100f;
        st.Speed = 5f;
        st.AtkSpeed = 1f;
        st.Stamina = 100f;
        st.Strength = 1f;
        st.IsInvincible = false;
        return st;
    }
}
