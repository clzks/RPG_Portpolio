using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : IData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float Hp { get; set; }
    public float ChaseSpeed { get; set; }
    public float PatrolSpeed { get; set; }
    public float Damage { get; set; }
    public float AttackRange { get; set; }
    public float AttackTerm { get; set; }
    public float DetectionDistance { get; set; }
    public float ChaseDistance { get; set; }
    public float PatrolCycle { get; set; }

    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Name;
    }
}
