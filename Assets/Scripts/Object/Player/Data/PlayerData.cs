using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : IData
{
    public List<int> EquipmentList { get; set; }
    public List<SkillInfo> SkillList { get; set; }
    public List<string> SkillSlots { get; set; }
    public int Level { get; set; }
    public Status Status { get; set; }
    public int Gold { get; set; }
    public int Exp { get; set; }
    public int SkillPoint { get; set; }
    public Dictionary<ItemType, SortedList<int,int>> Inventory { get; set; }

    public static PlayerData MakeNewPlayerData()
    {
        PlayerData data = new PlayerData();
        data.EquipmentList = new List<int>();
        data.EquipmentList.Add(-1);
        data.EquipmentList.Add(-1);
        data.EquipmentList.Add(-1);
        data.SkillList = new List<SkillInfo>();
        data.SkillSlots = new List<string>();
        //data.SkillSlots.Add(-1);
        //data.SkillSlots.Add(-1);
        //data.SkillSlots.Add(-1);
        data.Inventory = new Dictionary<ItemType, SortedList<int, int>>();
        data.Inventory.Add(ItemType.Weapon, new SortedList<int, int>());
        data.Inventory.Add(ItemType.Armor, new SortedList<int, int>());
        data.Inventory.Add(ItemType.Accessory, new SortedList<int, int>());
        data.Inventory.Add(ItemType.Consumable, new SortedList<int, int>());
        data.Inventory.Add(ItemType.Quest, new SortedList<int, int>());
        data.Status = Status.MakeSampleStatus();
        data.Level = 1;
        data.Gold = 0;
        data.Exp = 0;
        data.SkillPoint = 0;
        return data;
    }

    public int GetId()
    {
        return 0;
    }

    public string GetName()
    {
        return string.Empty;
    }
}
