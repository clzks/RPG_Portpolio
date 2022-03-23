using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectInfo : IData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float Life { get; set; }
    public bool IsRotate { get; set; }

    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Name;
    }
}
