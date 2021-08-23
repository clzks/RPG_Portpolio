using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMap : MonoBehaviour, IPoolObject
{
    private string _name;
    private Vector3 startPosition;
    private ObjectPoolManager _poolManager;

    private void OnEable()
    {
        if(null == _poolManager)
        {
            _poolManager = ObjectPoolManager.Get();
        }

        Init();
    }

    public string GetName()
    {
        return _name;
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public ObjectType GetObjectType()
    {
        return ObjectType.Map;
    }

    public void Init()
    {
        var pointList = GetComponentsInChildren<MapPoint>();
        startPosition = pointList[0].transform.position;

        foreach (MapPoint point in pointList)
        {
            if (point.EventType == MapEventType.NormalMonster)
            {
                foreach (var info in point.SummonList)
                {
                    for (int i = 0; i < info.count; ++i) 
                    {
                        Vector3 randomPoint = transform.position + Random.insideUnitSphere * point.SummonMaxRange;
                        var enemy = _poolManager.MakeObject(info.id, ObjectType.Enemy).GetComponent<BaseEnemy>();
                        enemy.MakeSampleStatus();
                        enemy.transform.position = randomPoint;
                    }
                }
            }
            else
            {
                return;
            }
        }

        transform.position = new Vector3(0, 0, 0);

    }

    public void ReturnObject()
    {
    
    }
}
