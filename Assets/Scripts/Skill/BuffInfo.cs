using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffInfo : IData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public BuffType Type { get; set; }
    public CalculateType CalculateType { get; set; }
    public float Value { get; set; }
    public float Life { get; set; }
    public float Tick { get; set; }
    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Name;
    }
}
