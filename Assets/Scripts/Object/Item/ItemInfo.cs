using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : IData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public Status Values { get; set; }
    public ItemType Type { get; set; }
    public ItemClassType Class { get; set; }
    public int PurchasePrice { get; set; }
    public int SellPrice { get; set; }
    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Name;
    }
}
