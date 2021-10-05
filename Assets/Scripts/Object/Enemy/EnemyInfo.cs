using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : IData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Status Status { get; set; }
    public List<int> DropItemList { get; set; }
    public float DropItemPercentage { get; set; }
    public int MinGold { get; set; }
    public int MaxGold { get; set; }
    public float DropGoldPercentage { get; set; }
    public int Exp { get; set; }

    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Name;
    }
}
