using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    public float CurrHp;
    public float MaxHp;
    public float Speed;
    public float AttackSpeed;
    public float Stamina;
    public float Strength;
    public float Damage;

    public float ChaseSpeed;
    public float PatrolSpeed;
    public float AttackRange;
    public float AttackTerm;
    public float DetectionDistance;
    public float ChaseDistance;
    public float PatrolCycle;
    public bool IsInvincible;


    public static Status MakeSampleStatus()
    {
        Status st = new Status();
        st.MaxHp = 100f;
        st.CurrHp = st.MaxHp;
        st.Speed = 5f;
        st.AttackSpeed = 1f;
        st.Stamina = 100f;
        st.Strength = 1f;
        st.IsInvincible = false;
        return st;
    }

    public void CopyStatus(Status status)
    {
        AttackRange = status.AttackRange;
        AttackSpeed = status.AttackSpeed;
        AttackTerm = status.AttackTerm;
        ChaseDistance = status.ChaseDistance;
        ChaseSpeed = status.ChaseSpeed;
        CurrHp = status.CurrHp;
        Damage = status.Damage;
        DetectionDistance = status.DetectionDistance;
        IsInvincible = status.IsInvincible;
        MaxHp = status.MaxHp;
        PatrolCycle = status.PatrolCycle;
        PatrolSpeed = status.PatrolSpeed;
        Speed = status.Speed;
        Stamina = status.Stamina;
        Strength = status.Strength;
    }
}
