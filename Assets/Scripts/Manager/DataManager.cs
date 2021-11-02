using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private PlayerData _playerData;
    private Dictionary<string, ActionInfo> _actionInfoList;
    private Dictionary<string, Dictionary<string,EnemyAction>> _enemyActionList;
    private Dictionary<int, EnemyInfo> _enemyInfoList;
    private Dictionary<int, MapInfo> _mapInfoList;
    private Dictionary<int, BuffInfo> _buffInfoList;
    private Dictionary<int, ItemInfo> _itemInfoList;
    private Dictionary<string, EffectInfo> _effectInfoList;
    private Dictionary<string, Sprite> _skillImageList;

    private void Awake()
    {
        _actionInfoList = new Dictionary<string, ActionInfo>();
        _enemyActionList = new Dictionary<string, Dictionary<string, EnemyAction>>();
        _enemyInfoList = new Dictionary<int, EnemyInfo>();
        _mapInfoList = new Dictionary<int, MapInfo>();
        _buffInfoList = new Dictionary<int, BuffInfo>();
        _itemInfoList = new Dictionary<int, ItemInfo>();
        _effectInfoList = new Dictionary<string, EffectInfo>();
        _skillImageList = new Dictionary<string, Sprite>();
    }

    public async UniTask<bool> LoadPlayerActionList()
    {
        _actionInfoList = await JsonConverter<ActionInfo>.GetJsonToDictionaryKeyName();
        if(null != _actionInfoList)
        {
            Debug.Log("플레이어 액션리스트 읽기 성공");
            return true;
        }
        else
        {
            Debug.Log("플레이어 액션리스트 읽기 실패");
            return false;
        }
    }

    public async UniTask<bool> LoadEnemyActionList()
    {
        Dictionary<int, EnemyAction> enemyActions = new Dictionary<int, EnemyAction>();
        enemyActions = await JsonConverter<EnemyAction>.GetJsonToDictionaryKeyId();

        foreach (var item in enemyActions.Values)
        {
            if(false == _enemyActionList.ContainsKey(item.CharacterName))
            {
                Dictionary<string, EnemyAction> actionList = new Dictionary<string, EnemyAction>();
                _enemyActionList.Add(item.CharacterName, actionList);
            }

            _enemyActionList[item.CharacterName].Add(item.Name, item);
        }
        
        if(null != _enemyActionList)
        {
            Debug.Log("이네미 액션리스트 읽기 성공");
            return true;
        }
        else
        {
            Debug.Log("이네미 액션리스트 읽기 실패");
            return false;
        }
    }
    
    public async UniTask<bool> LoadEnemyInfoList()
    {
        _enemyInfoList = await JsonConverter<EnemyInfo>.GetJsonToDictionaryKeyId();

        if (null != _actionInfoList)
        {
            Debug.Log("적군 데이터리스트 읽기 성공");
            return true;
        }
        else
        {
            Debug.Log("적군 데이터리스트 읽기 실패");
            return false;
        }
    }

    public async UniTask<bool> LoadMapInfoList()
    {
        _mapInfoList = await JsonConverter<MapInfo>.GetJsonToDictionaryKeyId();

        if (null != _mapInfoList)
        {
            Debug.Log("맵 정보 읽기 성공");
            return true;
        }
        else
        {
            Debug.Log("맵 정보 읽기 실패");
            return false;
        }
    }

    public async UniTask<bool> LoadBuffInfoList()
    {
        _buffInfoList = await JsonConverter<BuffInfo>.GetJsonToDictionaryKeyId();

        if(null != _buffInfoList)
        {
            Debug.Log("버프 정보 읽기 성공");
            return true;
        }
        else
        {
            Debug.Log("버프 정보 읽기 실패");
            return false;
        }
    }

    public async UniTask<bool> LoadPlayerData()
    {
        _playerData = await JsonConverter<PlayerData>.LoadJson();

        // 불러오는 작업 후
        if (null == _playerData)
        {
            Debug.Log("플레이어 정보 없음. 플레이어 정보 새로 생성");
            MakeNewPlayerData();
            return false;
        }
        else
        {
            Debug.Log("플레이어 정보 읽기 성공");
            return true;
        }
    }

    public async UniTask<bool> LoadItemList()
    {
        _itemInfoList = await JsonConverter<ItemInfo>.GetJsonToDictionaryKeyId();

        if (null != _itemInfoList)
        {
            Debug.Log("아이템 정보 읽기 성공");
            return true;
        }
        else
        {
            Debug.Log("아이템 정보 읽기 실패");
            return false;
        }
    }

    public async UniTask<bool> LoadEffectList()
    {
        _effectInfoList = await JsonConverter<EffectInfo>.GetJsonToDictionaryKeyName();

        if(null != _effectInfoList)
        {
            Debug.Log("이펙트 정보 읽기 성공");
            return true;
        }
        else
        {
            Debug.Log("이펙트 정보 읽기 실패");
            return false;
        }
    }

    public bool LoadSkillImageList()
    {
        foreach (var item in _actionInfoList.Values)
        {
            if(ActionType.Skill == item.Type)
            {
                var image = Resources.Load<Sprite>("Png/Skills/" + item.Name);
                if(null == image)
                {
                    Debug.Log("스킬 이미지 불러오기 실패");
                    return false;
                }
                _skillImageList.Add(item.Name, image);
            }
        }

        Debug.Log("스킬 이미지 불러오기 성공");
        return true;
    }

    public void MakeNewPlayerData()
    {
        _playerData = PlayerData.MakeNewPlayerData();
        SavePlayerData();
    }

    public void SavePlayerData()
    {
        JsonConverter<PlayerData>.WriteJson(_playerData);
    }

    public Dictionary<string, ActionInfo> GetActionInfoList()
    {
        return _actionInfoList;
    }

    public ActionInfo GetActionInfo(string name)
    {
        return _actionInfoList[name];
    }

    public EnemyAction GetEnemyActionInfo(string enemyName, string actionName)
    {
        return _enemyActionList[enemyName][actionName];
    }

    public Dictionary<int, EnemyInfo> GetEnemyInfoList()
    {
        return _enemyInfoList;
    }

    public EnemyInfo GetEnemyInfo(int id)
    {
        return _enemyInfoList[id];
    }

    public Dictionary<int, MapInfo> GetMapInfoList()
    {
        return _mapInfoList;
    }

    public MapInfo GetMapInfo(int id)
    {
        return _mapInfoList[id];
    }

    public BuffInfo GetBuffInfo(int id)
    {
        return _buffInfoList[id];
    }

    public PlayerData GetPlayerData()
    {
        return _playerData;
    }

    public void SetPlayerData(PlayerData data)
    {
        _playerData = data;
    }

    public ItemInfo GetItemInfo(int id)
    {
        if(-1 == id)
        {
            return null;
        }

        return _itemInfoList[id];
    }

    public Dictionary<string, EffectInfo> GetEffectInfoList()
    {
        return _effectInfoList;
    }

    public EffectInfo GetEffectInfo(string key)
    {
        return _effectInfoList[key];
    }

    public Sprite GetSkillImage(string key)
    {
        return _skillImageList[key];
    }
}
