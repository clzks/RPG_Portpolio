using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class HitUnitInfo
{
    public float ColliderRadius;
    [Tooltip("본체 전방으로의 거리")]
    public float FrontPos;
    [Tooltip("본체 측면상의 거리")]
    public float SidePos;
    public float Life;
    public float DamageFactor;
    public float StrengthFactor;
    public int Layer;
}
