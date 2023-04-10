using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingData : IData
{
    public bool IsHoming { get; set; }
    public bool IsSkillDirection { get; set; }
    public bool IsFixStick { get; set; }

    public int GetId()
    {
        return 0;
    }

    public string GetName()
    {
        return ""; 
    }

    public void SetData(GameSettingType type, bool isOn)
    {
        switch (type)
        {
            case GameSettingType.Homing:
                IsHoming = isOn;
                break;
            case GameSettingType.SkillDirection:
                IsSkillDirection = isOn;
                break;
            case GameSettingType.FixStick:
                IsFixStick = isOn;
                break;
        }
    }
}
