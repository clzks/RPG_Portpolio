using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionInfo : IData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CharacterName { get; set; }
    public int StartLevelValue { get; set; }
    public int RequiredOpenValue { get; set; }
    public int RequiredUpgradeValue { get; set; }
    public int MaxLevel { get; set; }
    public float Cost { get; set; }
    public int Exp { get; set; }
    public float CoolTime { get; set; }
    public float Factor { get; set; }
    public float LevelPerFactor { get; set; }
    //public int BuffId { get; set; }
    // public bool IsLearn { get; set; } 필요 없을듯. 1레벨 이상이 배운거나 마찬가지 이기 땜운
    public ActionType Type { get; set; }
    public float ComboAvailableTime { get; set; }
    public float MoveStartTime { get; set; }
    public float MoveTime { get; set; }
    public float MoveDistance { get; set; }
    public float AnimationStartTime { get; set; }   
    public float AnimationEndTime { get; set; }
    public List<HitUnitInfo> HitUnitList { get; set; }
    public bool DuplicatedHit { get; set; }
    public bool IsCustomPositon { get; set; }
    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Name;
    }

    public SkillInfo ConvertSkillInfo()
    {
        SkillInfo info = new SkillInfo
        {
            name = Name,
            id = Id,
            level = StartLevelValue
        };

        return info;
    }

    public SkillInfo ConvertSkillInfo(int levelValue)
    {
        SkillInfo info = new SkillInfo
        {
            name = Name,
            id = Id,
            level = levelValue
        };

        return info;
    }

    public int CalculateNextLevel(int currLevel)
    {
        if (0 == currLevel)
        {
            return RequiredOpenValue;
        }
        else
        {
            if (currLevel == MaxLevel)
            {
                return -1;
            }
            else
            {
                return RequiredOpenValue + currLevel * RequiredUpgradeValue;
            }
        }
    }
}

public struct SkillInfo
{
    public string name;
    public int id;
    public int level;
}