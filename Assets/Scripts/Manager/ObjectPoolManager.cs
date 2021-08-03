using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.U2D;
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public Dictionary<ObjectType, GameObject> prefabList;
    public Dictionary<ObjectType, List<GameObject>> _objectPoolList;
    public Dictionary<ObjectType, List<GameObject>> _activePoolList;
    private void Awake()
    {
        prefabList = new Dictionary<ObjectType, GameObject>();

    }

    public IPoolObject MakeObject(ObjectType type)
    {
        IPoolObject obj;

        List<GameObject> pool;
        List<GameObject> activePool;

        if (_objectPoolList.ContainsKey(type) == true)
        {
            pool = _objectPoolList[type];
        }
        else
        {
            pool = new List<GameObject>();
            _objectPoolList.Add(type, pool);
        }

        if (pool.Count != 0)
        {
            obj = pool[0].GetComponent<IPoolObject>();
            obj.GetObject().SetActive(true);
            pool.Remove(obj.GetObject());
        }
        else
        {
            obj = Instantiate(prefabList[type]).GetComponent<IPoolObject>();
        }

        if (_activePoolList.ContainsKey(type) == true)
        {
            activePool = _activePoolList[type];
        }
        else
        {
            activePool = new List<GameObject>();
            _activePoolList.Add(type, activePool);
        }

        obj.Init();
        activePool.Add(obj.GetObject());

        return obj;
    }

    public void ReturnObject(IPoolObject poolObject)
    {
        ObjectType type = poolObject.GetObjectType();
        var list = _activePoolList[type];
        var pool = _objectPoolList[type];
        GameObject obj = poolObject.GetObject();

        if (true == list.Contains(obj))
        {
            obj.transform.rotation = new Quaternion();
            obj.SetActive(false);
            list.Remove(obj);
            pool.Add(obj);
        }
    }

    public void InitPool()
    {
        _objectPoolList = new Dictionary<ObjectType, List<GameObject>>();
        _activePoolList = new Dictionary<ObjectType, List<GameObject>>();
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
