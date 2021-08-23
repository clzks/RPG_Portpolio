using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.U2D;
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    private DataManager _dataManager;
    public Dictionary<string, GameObject> prefabList;
    public Dictionary<string, List<GameObject>> _objectPoolList;
    public Dictionary<string, List<GameObject>> _activePoolList;
    private void Awake()
    {
        prefabList = new Dictionary<string, GameObject>();
        _dataManager = DataManager.Get();
    }

    public GameObject MakeObject(string objName)
    {
        IPoolObject obj;

        List<GameObject> pool;
        List<GameObject> activePool;

        if (_objectPoolList.ContainsKey(objName) == true)
        {
            pool = _objectPoolList[objName];
        }
        else
        {
            pool = new List<GameObject>();
            _objectPoolList.Add(objName, pool);
        }

        if (pool.Count != 0)
        {
            obj = pool[0].GetComponent<IPoolObject>();
            obj.GetObject().SetActive(true);
            pool.Remove(obj.GetObject());
        }
        else
        {
            obj = Instantiate(prefabList[objName]).GetComponent<IPoolObject>();
        }

        if (_activePoolList.ContainsKey(objName) == true)
        {
            activePool = _activePoolList[objName];
        }
        else
        {
            activePool = new List<GameObject>();
            _activePoolList.Add(objName, activePool);
        }

        obj.Init();
        activePool.Add(obj.GetObject());

        return obj.GetObject();
    }

    public GameObject MakeObject(int id, ObjectType type)
    {
        string objName = string.Empty;

        if (ObjectType.Enemy == type)
        {
            objName = _dataManager.GetEnemyInfoList()[id].Name;
        }
        else if(ObjectType.Map == type)
        {
            objName = _dataManager.GetMapInfoList()[id].Name;
        }
        else
        {
            Debug.LogError("생산할 수 없는 방식입니다");
            return null;
        }

        if(string.Empty == objName)
        {
            Debug.LogError("잘못된 Id를 입력하였습니다");
            return null;
        }

        IPoolObject obj;

        List<GameObject> pool;
        List<GameObject> activePool;

        if (_objectPoolList.ContainsKey(objName) == true)
        {
            pool = _objectPoolList[objName];
        }
        else
        {
            pool = new List<GameObject>();
            _objectPoolList.Add(objName, pool);
        }

        if (pool.Count != 0)
        {
            obj = pool[0].GetComponent<IPoolObject>();
            obj.GetObject().SetActive(true);
            pool.Remove(obj.GetObject());
        }
        else
        {
            obj = Instantiate(prefabList[objName]).GetComponent<IPoolObject>();
        }

        if (_activePoolList.ContainsKey(objName) == true)
        {
            activePool = _activePoolList[objName];
        }
        else
        {
            activePool = new List<GameObject>();
            _activePoolList.Add(objName, activePool);
        }

        obj.Init();
        activePool.Add(obj.GetObject());

        return obj.GetObject();
    }

    public void ReturnObject(IPoolObject poolObject)
    {
        string name = poolObject.GetName();
        var list = _activePoolList[name];
        var pool = _objectPoolList[name];
        GameObject obj = poolObject.GetObject();

        if (true == list.Contains(obj))
        {
            obj.transform.rotation = new Quaternion();
            obj.SetActive(false);
            list.Remove(obj);
            pool.Add(obj);
        }
    }

    public void LoadPrefabs()
    {
        LoadEnemyPrefab();
        LoadHitUnit();
        LoadMapPrefab();
    }
    #region LoadPrefab
    public void LoadEnemyPrefab()
    {
        var list = DataManager.Get().GetEnemyInfoList();

        foreach (var item in list)
        {
            EnemyInfo info = item.Value;
            var obj = Resources.Load<GameObject>("Prefabs/Enemy/" + info.Name);
            prefabList.Add(info.Name, obj);
        }
    }

    public void LoadHitUnit()
    {
        var obj = Resources.Load<GameObject>("Prefabs/HitUnit/NormalHitUnit");
        prefabList.Add("NormalHitUnit", obj);
    }

    public void LoadMapPrefab()
    {
        var list = DataManager.Get().GetMapInfoList();

        foreach (var item in list)
        {
            MapInfo info = item.Value;
            var obj = Resources.Load<GameObject>("Prefabs/Map/" + info.Name);
            prefabList.Add(info.Name, obj);
        }
    }
    #endregion
    public void InitPool()
    {
        _objectPoolList = new Dictionary<string, List<GameObject>>();
        _activePoolList = new Dictionary<string, List<GameObject>>();
    }

    public void ReturnAllObject()
    {
        foreach (var item in _activePoolList)
        {
            for (int i = item.Value.Count - 1; i > -1; --i)
            {
                IPoolObject iObj = item.Value[i].GetComponent<IPoolObject>();
                ReturnObject(iObj);
            }
        }
    }

    public void ResetPool()
    {
        foreach (var item in _objectPoolList)
        {
            foreach (var obj in item.Value)
            {
                Destroy(obj);
            }

            item.Value.Clear();
        }

        foreach (var item in _activePoolList)
        {
            foreach (var obj in item.Value)
            {
                Destroy(obj);
            }

            item.Value.Clear();
        }
    }
}
