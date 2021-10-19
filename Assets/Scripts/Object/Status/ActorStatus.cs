using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    public float CurrHp { get; set; }
    public float MaxHp { get; set; }
    public float Speed { get; set; }
    public float AttackSpeed { get; set; }
    public float Stamina { get; set; }
    public float Strength { get; set; }
    public float Defence { get; set; }
    public float Damage { get; set; }

    public float ChaseSpeed { get; set; }
    public float PatrolSpeed { get; set; }
    public float AttackRange { get; set; }
    public float AttackTerm { get; set; }
    public float DetectionDistance { get; set; }
    public float ChaseDistance { get; set; }
    public float PatrolCycle { get; set; }
    public bool IsInvincible { get; set; }


    public static Status MakeSampleStatus()
    {
        Status st = new Status();
        st.MaxHp = 100f;
        st.CurrHp = st.MaxHp;
        st.Speed = 5f;
        st.AttackSpeed = 1f;
        st.Stamina = 100f;
        st.Defence = 0f;
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
