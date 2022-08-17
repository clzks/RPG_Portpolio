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
    public float MaxStamina { get; set; }
    public float Stamina { get; set; }
    public float RegenStamina { get; set; }
    public float Strength { get; set; }
    public float Defence { get; set; }
    public float Attack { get; set; }
    public float Shield { get; set; }
    public float ChaseSpeed { get; set; }
    public float PatrolSpeed { get; set; }
    public float AttackRange { get; set; }
    public float AttackTerm { get; set; }
    public float DetectionDistance { get; set; }
    public float ChaseDistance { get; set; }
    public float PatrolCycle { get; set; }


    public static Status MakeSampleStatus()
    {
        Status st = new Status();
        st.MaxHp = 100f;
        st.CurrHp = st.MaxHp;
        st.Speed = 5f;
        st.AttackSpeed = 1f;
        st.MaxStamina = 100f;
        st.Stamina = 100f;
        st.RegenStamina = 1f;
        st.Defence = 0f;
        st.Strength = 1f;
        return st;
    }

    public static Status CopyStatus(Status status)
    {
        Status s = new Status();

        s.AttackRange = status.AttackRange;
        s.AttackSpeed = status.AttackSpeed;
        s.AttackTerm = status.AttackTerm;
        s.ChaseDistance = status.ChaseDistance;
        s.ChaseSpeed = status.ChaseSpeed;
        s.Shield = status.Shield;
        s.CurrHp = status.CurrHp;
        s.MaxHp = status.MaxHp;
        s.Attack = status.Attack;
        s.Defence = status.Defence;
        s.DetectionDistance = status.DetectionDistance;
        s.PatrolCycle = status.PatrolCycle;
        s.PatrolSpeed = status.PatrolSpeed;
        s.Speed = status.Speed;
        s.Stamina = status.Stamina;
        s.MaxStamina = status.MaxStamina;
        s.Strength = status.Strength;

        return s;
    }
}
