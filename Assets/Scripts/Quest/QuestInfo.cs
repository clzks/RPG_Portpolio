using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInfo : IData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public QuestType Type { get; set; }
    public int QuestTargetId { get; set; }
    public int QuestValue { get; set; }
    public int MapId { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public string ClearDescription { get; set; }
    public string Client { get; set; }
    public int NextQuestId { get; set; }
    public List<QuestReward> Rewards { get; set; }

    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Name;
    }
}
