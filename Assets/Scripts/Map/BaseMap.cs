using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class BaseMap : MonoBehaviour, IPoolObject
{
    private Player _player;
    private string _name;
    private int _id;
    private ObjectPoolManager _objectPool;
    private DataManager _dataManager;
    //private bool _isFirst = true;
    public NavMeshSurface surface;
    private List<MapPoint> _pointList;
    private void Awake()
    {
        _objectPool = ObjectPoolManager.Get();
        _dataManager = DataManager.Get();
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

    public void GetHeightMap()
    {
        TerrainData td = GetComponentInChildren<Terrain>().terrainData;

        var heights = td.GetHeights(0, 0, 100, 100);

        Texture2D t = new Texture2D(0,0);
    }

    public void Init()
    {
        GetHeightMap();
        //if (true == _isFirst)
        //{
        //    _isFirst = false;
        //    surface.RemoveData();
        //    surface.BuildNavMesh();
        //}
        _pointList = GetComponentsInChildren<MapPoint>().ToList();

        foreach (MapPoint point in _pointList)
        {
            // Summon Monster Point
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
            else if (point.EventType == MapEventType.UniqueMonster)
            {
                foreach (var info in point.SummonList)
                {
                    SummonDragon(info.id, point.transform.position);
                }
            }
            else
            {
                continue;
            }
        }

        transform.position = new Vector3(0, 0, 0);

    }

    public void SetMap(MapInfo info)
    {
        _id = info.Id;
        _name = info.Name;
    }
        
    public void SetName(string name)
    {
        _name = name;
    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    public Vector3 GetPointPosition(int pointIndex)
    {
        return _pointList[pointIndex].transform.position;
    }

    public void ReturnObject()
    {
        _objectPool.ReturnObject(this);
    }

    public void SummonNormalEnemy(int id, Vector3 summonPos, Transform baseCamp)
    {
        var enemy = _objectPool.MakeObject(ObjectType.Enemy, id).GetComponent<BaseEnemy>();
        enemy.SetEnemy(_dataManager.GetEnemyInfo(id), new EnemyIdleState(enemy));
        enemy.SetPlayer(_player);
        enemy.SetBaseCamp(baseCamp);
        enemy.transform.position = summonPos;
        enemy.SetActiveNavMeshAgent(true);
    }

    public void SummonDragon(int id, Vector3 summonPos)
    {
        var boss = _objectPool.MakeObject(ObjectType.Enemy, id).GetComponent<BaseEnemy>();
        boss.SetEnemy(_dataManager.GetEnemyInfo(id), new DragonReadyState(boss));
        boss.SetPlayer(_player);
        //enemy.SetBaseCamp(baseCamp);
        boss.transform.position = summonPos;
        boss.SetActiveNavMeshAgent(true);
    }

    public Vector3 GetPosition()
    {
        return new Vector3();
    }
}
