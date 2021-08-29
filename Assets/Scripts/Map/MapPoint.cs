using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPoint : MonoBehaviour
{
    public int Index;
    public bool IsLock = false;
    [DrawIf("IsLock", true)] public int UnlockCondition;
    [DrawIf("EventType", MapEventType.Transition)] public TrasitionInfo TransInfo;
    public MapEventType EventType = MapEventType.Count;
    public List<SummonInfo> SummonList;
    [DrawIf("EventType", MapEventType.NormalMonster)] public float SummonMaxRange;
    private BattleScene _scene;
    private void Awake()
    {
        _scene = GameObject.Find("Scripts").GetComponent<BattleScene>();
    }

    public void OnDrawGizmos()
    {
        if(EventType == MapEventType.NormalMonster)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawSphere(transform.position, SummonMaxRange);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(true == IsLock)
        {
            return;
        }

        if (true == other.CompareTag("Player"))
        {
            if (MapEventType.Transition == EventType)
            {
                _scene.EnterNewWorld(TransInfo.mapId, TransInfo.mapPointId);
            }
        }
    }
}

[System.Serializable]
public struct SummonInfo
{
    public int id;
    public int count;
}

[System.Serializable]
public struct TrasitionInfo
{
    public int mapId;
    public int mapPointId;
}
