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
    private ScenarioManager _scenarioManager;
    [SerializeField]private SphereCollider _collider;
    [SerializeField]private SpriteRenderer _renderer;
    private void Awake()
    {
        _scene = GameObject.Find("Scripts").GetComponent<BattleScene>();
        //_collider = gameObject.GetComponent<SphereCollider>();
    }

    public void SetActiveRenderer(bool enabled)
    {
        _renderer.enabled = enabled;
    }

    public void OnDrawGizmos()
    {
        if(EventType == MapEventType.NormalMonster)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawSphere(transform.position, SummonMaxRange);
        }
        else if(EventType == MapEventType.Transition)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, _collider.radius);
        }
        else if(EventType == MapEventType.SummonPoint)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, _collider.radius);
        }
        else if(EventType == MapEventType.UniqueMonster)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, _collider.radius);
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
            else if(MapEventType.QuestDestination == EventType)
            {
                _scene.PlayerEnterMapPoint(this);
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
