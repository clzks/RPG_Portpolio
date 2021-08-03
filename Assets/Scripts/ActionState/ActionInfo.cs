using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionInfo : IData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CharacterName { get; set; }
    public float ComboAvailableTime { get; set; }
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
