using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPoint : MonoBehaviour
{
    public int Index;
    public bool IsLock;
    public int UnlockCondition;
    public MapEventType EventType;
    public List<SummonInfo> SummonList;
    public float SummonMaxRange;

    public void OnDrawGizmos()
    {
        if(EventType == MapEventType.NormalMonster)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawSphere(transform.position, SummonMaxRange);
        }
    }
}

[System.Serializable]
public struct SummonInfo
{
    public int id;
    public int count;
}