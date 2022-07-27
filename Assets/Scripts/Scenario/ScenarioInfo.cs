using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioInfo : IData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ConditionPointIndex { get; set; }
    public int ReadyDialogInfoId { get; set; }
    public int QuestId { get; set; }
    public int ClearDialogInfoId { get; set; }
    public int NextScenarioId { get; set; }
    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Name;
    }
}
