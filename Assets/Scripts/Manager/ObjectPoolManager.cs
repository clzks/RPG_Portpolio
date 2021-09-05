using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.U2D;
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    private DataManager _dataManager;
    public Dictionary<string, GameObject> prefabList;
    public Dictionary<ObjectType, List<IPoolObject>> _objectPoolList;
    public Dictionary<ObjectType, List<IPoolObject>> _activePoolList;
    public Dictionary<string, Material> _materialList;
    private void Awake()
    {
        prefabList = new Dictionary<string, GameObject>();
        _materialList = new Dictionary<string, Material>();
        _dataManager = DataManager.Get();
    }

    public void LoadPrefabs()
    {
        LoadEnemyPrefab();
        LoadHitUnit();
        LoadMapPrefab();
        LoadMaterials();
    }

    public GameObject MakeObject(ObjectType type, string objName)
    {
        IPoolObject obj;

        List<IPoolObject> pool;
        List<IPoolObject> activePool;

        if (_objectPoolList.ContainsKey(type) == true)
        {
            pool = _objectPoolList[type];
        }
        else
        {
            pool = new List<IPoolObject>();
            _objectPoolList.Add(type, pool);
        }

        if (pool.Count != 0)
        {
            obj = pool.FirstOrDefault(x => x.GetName() == objName);

            if (null != obj)
            {
                obj.GetObject().SetActive(true);
                pool.Remove(obj);
            }
            else
            {
                obj = Instantiate(prefabList[objName]).GetComponent<IPoolObject>();
            }
        }
        else
        {
            obj = Instantiate(prefabList[objName]).GetComponent<IPoolObject>();
        }

        if (_activePoolList.ContainsKey(type) == true)
        {
            activePool = _activePoolList[type];
        }
        else
        {
            activePool = new List<IPoolObject>();
            _activePoolList.Add(type, activePool);
        }

        activePool.Add(obj);

        return obj.GetObject();
    }

    public GameObject MakeObject(ObjectType type, int id)
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

        List<IPoolObject> pool;
        List<IPoolObject> activePool;

        if (_objectPoolList.ContainsKey(type) == true)
        {
            pool = _objectPoolList[type];
        }
        else
        {
            pool = new List<IPoolObject>();
            _objectPoolList.Add(type, pool);
        }

        if (pool.Count != 0)
        {
            obj = pool.FirstOrDefault(x => x.GetName() == objName);

            if (null != obj)
            {
                obj.GetObject().SetActive(true);
                pool.Remove(obj);
            }
            else
            {
                obj = Instantiate(prefabList[objName]).GetComponent<IPoolObject>();
            }
        }
        else
        {
            obj = Instantiate(prefabList[objName]).GetComponent<IPoolObject>();
        }

        if (_activePoolList.ContainsKey(type) == true)
        {
            activePool = _activePoolList[type];
        }
        else
        {
            activePool = new List<IPoolObject>();
            _activePoolList.Add(type, activePool);
        }

        activePool.Add(obj);

        return obj.GetObject();
    }

    public void ReturnObject(IPoolObject poolObject)
    {
        ObjectType type = poolObject.GetObjectType();
        var list = _activePoolList[type];
        var pool = _objectPoolList[type];
        //GameObject obj = poolObject.GetObject();

        if (true == list.Contains(poolObject))
        {
            GameObject obj = poolObject.GetObject();
            obj.transform.rotation = new Quaternion();
            obj.SetActive(false);
            list.Remove(poolObject);
            pool.Add(poolObject);
        }
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

    private void LoadMaterials()
    {
        var normal = Resources.Load<Material>("Materials/DesertRock_Normal");
        _materialList.Add("DesertRock_Normal", normal);
        var trans = Resources.Load<Material>("Materials/DesertRock_TransParent");
        _materialList.Add("DesertRock_TransParent", trans);
    }
    #endregion
    public void InitPool()
    {
        _objectPoolList = new Dictionary<ObjectType, List<IPoolObject>>();
        _activePoolList = new Dictionary<ObjectType, List<IPoolObject>>();
    }

    public void ReturnAllObject()
    {
        foreach (var item in _activePoolList)
        {
            for (int i = item.Value.Count - 1; i > -1; --i)
            {
                IPoolObject iObj = item.Value[i];
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
                Destroy(obj.GetObject());
            }

            item.Value.Clear();
        }

        foreach (var item in _activePoolList)
        {
            foreach (var obj in item.Value)
            {
                Destroy(obj.GetObject());
            }

            item.Value.Clear();
        }
    }

    public Material GetMaterial(string name)
    {
        return _materialList[name];
    }
}
