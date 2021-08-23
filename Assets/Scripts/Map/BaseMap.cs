using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseMap : MonoBehaviour, IPoolObject
{
    private Player _player;
    private string _name;
    private Vector3 _startPosition;
    private ObjectPoolManager _objectPool;
    private bool _isFirst = true;
    public NavMeshSurface surface;
    private void Awake()
    {
        _objectPool = ObjectPoolManager.Get();
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
        if (true == _isFirst)
        {
            _isFirst = false;
            surface.RemoveData();
            surface.BuildNavMesh();
        }
        var pointList = GetComponentsInChildren<MapPoint>();
        _startPosition = pointList[0].transform.position;

        foreach (MapPoint point in pointList)
        {
            if (point.EventType == MapEventType.NormalMonster)
            {
                foreach (var info in point.SummonList)
                {
                    for (int i = 0; i < info.count; ++i) 
                    {
                        Vector3 randomPoint = Random.insideUnitSphere * point.SummonMaxRange;
                        randomPoint = point.transform.position + new Vector3(randomPoint.x, 0f, randomPoint.z);
                        SummonNormalEnemy(info.id, randomPoint, point.transform);
                    }
                }
            }
            else
            {
                continue;
            }
        }

        transform.position = new Vector3(0, 0, 0);

    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    public Vector3 GetStartPosition()
    {
        return _startPosition;
    }

    public void ReturnObject()
    {
        _objectPool.ReturnObject(this);
    }

    public void SummonNormalEnemy(int id, Vector3 summonPos, Transform baseCamp)
    {
        var enemy = _objectPool.MakeObject(id, ObjectType.Enemy).GetComponent<BaseEnemy>();
        enemy.Init();
        enemy.SetPlayer(_player);
        enemy.SetBaseCamp(baseCamp);
        enemy.transform.position = summonPos;
        enemy.SetActiveNavMeshAgent(true);
    }
}
