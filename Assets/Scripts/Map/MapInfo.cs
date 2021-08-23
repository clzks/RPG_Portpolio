using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo : IData
{
    public string Name { get; set; }
    public int Id { get; set; }

    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Name;
    }
}
