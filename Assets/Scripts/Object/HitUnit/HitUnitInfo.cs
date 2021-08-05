using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class HitUnitInfo
{
    public float ColliderRadius;
    [Tooltip("��ü ���������� �Ÿ�")]
    public float FrontPos;
    [Tooltip("��ü ������� �Ÿ�")]
    public float SidePos;
    public float Life;
    public float DamageFactor;
    public float StrengthFactor;
    public int Layer;
}
