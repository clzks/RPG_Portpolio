using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.U2D;
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    private DataManager _dataManager;
    private Dictionary<string, GameObject> prefabList;
    private Dictionary<ObjectType, List<IPoolObject>> _objectPoolList;
    private Dictionary<ObjectType, List<IPoolObject>> _activePoolList;
    private Dictionary<string, Material> _materialList;
    private Dictionary<string, Sprite> _spriteList;
    private Dictionary<string, GameObject> _skillEffectList;
    private void Awake()
    {
        prefabList = new Dictionary<string, GameObject>();
        _materialList = new Dictionary<string, Material>();
        _spriteList = new Dictionary<string, Sprite>();
        _skillEffectList = new Dictionary<string, GameObject>();
        _dataManager = DataManager.Get();
    }

    public void LoadPrefabs()
    {
        LoadEnemyPrefab();
        LoadHitUnit();
        LoadMapPrefab();
        LoadDamageText();
        LoadMaterials();
        LoadGroundItem();
        LoadInventroyIcon();
        LoadSkillEffects();
        LoadSkillSettingClickIcon();
        LoadBuffIcon();
        LoadRewardIcon();
    }
    
    public void LoadSprite()
    {
        LoadResourcesSprite("Apple");
        LoadResourcesSprite("Armor");
        LoadResourcesSprite("Bag");
        LoadResourcesSprite("Belts");
        LoadResourcesSprite("Book");
        LoadResourcesSprite("Boots");
        LoadResourcesSprite("Bow");
        LoadResourcesSprite("Bracers");
        LoadResourcesSprite("Cloaks");
        LoadResourcesSprite("Coins");
        LoadResourcesSprite("Gem");
        LoadResourcesSprite("Helmets");
        LoadResourcesSprite("Hp");
        LoadResourcesSprite("Ingots");
        LoadResourcesSprite("Meat");
        LoadResourcesSprite("Mp");
        LoadResourcesSprite("Necklace");
        LoadResourcesSprite("Pants");
        LoadResourcesSprite("Rings");
        LoadResourcesSprite("Scroll");
        LoadResourcesSprite("Shield");
        LoadResourcesSprite("Shoulders");
        LoadResourcesSprite("Sword");
    }

    private void LoadResourcesSprite(string name)
    {
        var sprite = Resources.Load<Sprite>("Sprites/" + name);

        _spriteList.Add(name, sprite);
    }

    public GameObject GetObject(string key)
    {
        return prefabList[key];
    }

    /// <summary>
    /// 한 타입에 한종류의 오브젝트만 존재할 경우에 사용한다. type과 오브젝트 네임은 동일해야 한다
    /// </summary>
    /// <param name="type">오브젝트 타입 = 오브젝트 이름</param>
    /// <returns></returns>
    public GameObject MakeObject(ObjectType type)
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

        var objName = type.ToString();

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
            //obj.transform.rotation = new Quaternion();
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

    private void LoadDamageText()
    {
        var text = Resources.Load<GameObject>("Prefabs/DamageText/DamageText");
        prefabList.Add("DamageText", text);
    }

    private void LoadGroundItem()
    {
        var obj = Resources.Load<GameObject>("Prefabs/Item/GroundItem");
        prefabList.Add("GroundItem", obj);
    }

    private void LoadInventroyIcon()
    {
        var obj = Resources.Load<GameObject>("Prefabs/Inventory/InventoryIcon");
        prefabList.Add("InventoryIcon", obj);
    }
    
    private void LoadSkillEffects()
    {
        var list = DataManager.Get().GetEffectInfoList();

        foreach (var item in list)
        {
            EffectInfo info = item.Value;
            var obj = Resources.Load<GameObject>("Prefabs/Skills/" + info.Name);
            prefabList.Add(info.Name, obj);
        }
    }

    private void LoadSkillSettingClickIcon()
    {
        var obj = Resources.Load<GameObject>("Prefabs/Skills/UI/SkillSettingClickIcon");
        prefabList.Add("SkillSettingClickIcon", obj);
    }

    private void LoadBuffIcon()
    {
        var obj = Resources.Load<GameObject>("Prefabs/BuffIcon/BuffIcon");
        prefabList.Add("BuffIcon", obj);
    }

    private void LoadRewardIcon()
    {
        var obj = Resources.Load<GameObject>("Prefabs/UI/RewardIcon");
        prefabList.Add("RewardIcon", obj);
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

    public List<IPoolObject> GetEnemies()
    {
        return _activePoolList[ObjectType.Enemy];
    }

    public BaseEnemy GetEnemy()
    {
        // 가장 가까운 적 찾기
        return null;
    }

    public Sprite GetSprite(string name)
    {
        return _spriteList[name];
    }
}
