using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionInfo : IData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CharacterName { get; set; }
    public int Level { get; set; }
    public int MaxLevel { get; set; }
    public float Cost { get; set; }
    public int Exp { get; set; }
    public int LevelPerExp { get; set; }
    public float Factor { get; set; }
    public int BuffId { get; set; }
    public float LevelPerFactor { get; set; }
    public bool IsLearn { get; set; }
    public ActionType Type { get; set; }
    public float ComboAvailableTime { get; set; }
    public float MoveStartTime { get; set; }
    public float MoveTime { get; set; }
    public float MoveDistance { get; set; }
    public float AnimationStartTime { get; set; }
    public float AnimationEndTime { get; set; }
    public List<HitUnitInfo> HitUnitList { get; set; }

    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Name;
    }
}
