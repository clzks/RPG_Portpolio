using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class DataManager : Singleton<DataManager>
{
    private ObjectPoolManager _objectPool;
    private PlayerData _playerData;
    private PlayerData _originPlayerData;
    private GameSettingData _gameSettingData;
    private GameSettingData _originGameSettingData;
    private Dictionary<string, ActionInfo> _actionInfoList;
    private Dictionary<string, Dictionary<string,EnemyAction>> _enemyActionList;
    private Dictionary<int, EnemyInfo> _enemyInfoList;
    private Dictionary<int, MapInfo> _mapInfoList;
    private Dictionary<int, BuffInfo> _buffInfoList;
    private Dictionary<int, ItemInfo> _itemInfoList;
    private Dictionary<string, EffectInfo> _effectInfoList;
    private Dictionary<string, Sprite> _skillImageList;
    private Dictionary<int, QuestInfo> _questInfoList;
    private Dictionary<int, ScenarioInfo> _scenarioInfoList;
    private Dictionary<int, DialogInfo> _dialogInfoList;


    private void Awake()
    {
        _objectPool = ObjectPoolManager.Get();
        _actionInfoList = new Dictionary<string, ActionInfo>();
        _enemyActionList = new Dictionary<string, Dictionary<string, EnemyAction>>();
        _enemyInfoList = new Dictionary<int, EnemyInfo>();
        _mapInfoList = new Dictionary<int, MapInfo>();
        _buffInfoList = new Dictionary<int, BuffInfo>();
        _itemInfoList = new Dictionary<int, ItemInfo>();
        _effectInfoList = new Dictionary<string, EffectInfo>();
        _skillImageList = new Dictionary<string, Sprite>();
        _questInfoList = new Dictionary<int, QuestInfo>();
        _scenarioInfoList = new Dictionary<int, ScenarioInfo>();
        _dialogInfoList = new Dictionary<int, DialogInfo>();
    }

    public async UniTask<bool> LoadPlayerActionList()
    {
        _actionInfoList = await JsonConverter<ActionInfo>.GetJsonToDictionaryKeyName(this);
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
        enemyActions = await JsonConverter<EnemyAction>.GetJsonToDictionaryKeyId(this);

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
        _enemyInfoList = await JsonConverter<EnemyInfo>.GetJsonToDictionaryKeyId(this);

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
        _mapInfoList = await JsonConverter<MapInfo>.GetJsonToDictionaryKeyId(this);

        if (null != _mapInfoList)
        {
            foreach (var item in _mapInfoList)
            {
                _objectPool.LoadMinimapTexture(item.Key);
            }

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
        _buffInfoList = await JsonConverter<BuffInfo>.GetJsonToDictionaryKeyId(this);

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

    public async UniTask LoadPlayerData()
    {
        _playerData = await JsonConverter<PlayerData>.LoadJsonFromPersistent(this);

        // 불러오는 작업 후
        if (null == _playerData)
        {
            Debug.Log("플레이어 정보 없음");
            //MakeNewPlayerData();
            //return false;
        }
        else
        {
            Debug.Log("플레이어 정보 읽기 성공");
            //return true;
        }
    }

    public async UniTask<bool> LoadOriginPlayerData()
    {
        _originPlayerData = await JsonConverter<PlayerData>.LoadJsonFromStreamingAssets(this, "OriginPlayerData.json");

        // 불러오는 작업 후
        if (null == _originPlayerData)
        {
            Debug.LogError("초기 플레이어 정보 읽기 실패");
            return false;
        }
        else
        {
            Debug.Log("초기 플레이어 정보 읽기 성공");
            return true;
        }
    }

    public async UniTask LoadGameSettingData()
    {
        _gameSettingData = await JsonConverter<GameSettingData>.LoadJsonFromPersistent(this);

        // 불러오는 작업 후
        if (null == _gameSettingData)
        {
            Debug.Log("게임설정 정보 없음");
            _gameSettingData = _originGameSettingData;
            //return false;
        }
        else
        {
            Debug.Log("게임설정  정보 읽기 성공");
            //return true;
        }
    }

    public async UniTask<bool> LoadOriginGameSettingData()
    {
        _originGameSettingData = await JsonConverter<GameSettingData>.LoadJsonFromStreamingAssets(this, "OriginGameSettingData.json");

        // 불러오는 작업 후
        if (null == _originGameSettingData)
        {
            Debug.LogError("초기 게임설정 정보 읽기 실패");
            return false;
        }
        else
        {
            Debug.Log("초기 게임설정 정보 읽기 성공");
            return true;
        }
    }

    public async UniTask<bool> LoadItemList()
    {
        _itemInfoList = await JsonConverter<ItemInfo>.GetJsonToDictionaryKeyId(this);

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
        _effectInfoList = await JsonConverter<EffectInfo>.GetJsonToDictionaryKeyName(this);

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
            if (ActionType.Skill == item.Type)
            {
                var image = Resources.Load<Sprite>("Png/Skills/" + item.Name);
                if (null == image)
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

    public async UniTask<bool> LoadQuestInfoList()
    {
        _questInfoList = await JsonConverter<QuestInfo>.GetJsonToDictionaryKeyId(this);

        if (null != _questInfoList)
        {
            Debug.Log("퀘스트 정보 읽기 성공");
            return true;
        }
        else
        {
            Debug.Log("퀘스트 정보 읽기 실패");
            return false;
        }
    }

    public async UniTask<bool> LoadScenarioInfoList()
    {
        _scenarioInfoList = await JsonConverter<ScenarioInfo>.GetJsonToDictionaryKeyId(this);

        if (null != _scenarioInfoList)
        {
            Debug.Log("시나리오 정보 읽기 성공");
            return true;
        }
        else
        {
            Debug.Log("시나리오 정보 읽기 실패");
            return false;
        }
    }

    public async UniTask<bool> LoadDialogInfoList()
    {
        _dialogInfoList = await JsonConverter<DialogInfo>.GetJsonToDictionaryKeyId(this);

        if (null != _dialogInfoList)
        {
            Debug.Log("대화 정보 읽기 성공");
            return true;
        }
        else
        {
            Debug.Log("대화 정보 읽기 실패");
            return false;
        }
    }

    public void MakeNewPlayerData()
    {
        _playerData = _originPlayerData;
    }

    public void MakeNewGameSettingData()
    {
        _gameSettingData = _originGameSettingData;
    }

    public void SavePlayerData()
    {
        JsonConverter<PlayerData>.WriteJson(_playerData);
    }

    public void SaveGameSettingData()
    {
        JsonConverter<GameSettingData>.WriteJson(_gameSettingData);
    }

    public Dictionary<string, ActionInfo> GetActionInfoList()
    {
        return _actionInfoList;
    }

    public void SetPlayerSkillList(PlayerData data)
    {
        //data.SkillSlots.Add(string.Empty);
        //data.SkillSlots.Add(string.Empty);
        //data.SkillSlots.Add(string.Empty);

        //foreach (var info in _actionInfoList.Values)
        //{
        //    if(info.Type == ActionType.Skill)
        //    {
        //        data.SkillList.Add(info.ConvertSkillInfo());
        //    }
        //}

        data.SkillList = data.SkillList.OrderBy(x => x.id).ToList();
    }

    public ActionInfo GetActionInfo(string name)
    {
        if (name == string.Empty)
        {
            return null;
        }

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

    public GameSettingData GetGameSettingData()
    {
        return _gameSettingData;
    }

    public void SetGameSettingData(GameSettingData data)
    {
        _gameSettingData = data;
    }

    public void SetGameSettingData(GameSettingType type, bool isOn)
    {
        _gameSettingData.SetData(type, isOn);
    }

    public void SetPlayerSkillSlot(int i, string action)
    {
        var skillSlots = _playerData.SkillSlots;
        
        if(string.Empty == action)
        {
            skillSlots[i] = action;
        }

        var index = skillSlots.FindIndex(x => x == action);
        
        if(-1 == index)
        {
            skillSlots[i] = action;
        }
        else
        {
            if(i == index)
            {
                return;
            }
            else
            {
                ChangeSkillSlot(i, index);
            }
        }
    }

    private void ChangeSkillSlot(int start, int end)
    {
        var skillSlots = _playerData.SkillSlots;
        var startSlot = skillSlots[start];
        var endSlot = skillSlots[end];

        var tempSlot = endSlot;
        skillSlots[end] = startSlot;
        skillSlots[start] = tempSlot;
    }

    public string GetSkillSlot(int index)
    {
        return _playerData.SkillSlots[index];
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
        if(key == string.Empty)
        {
            return null;
        }

        return _skillImageList[key];
    }

    public Dictionary<int, QuestInfo> GetQuestInfoList()
    {
        return _questInfoList;
    }

    public Dictionary<int, ScenarioInfo> GetScenarioinfoList()
    {
        return _scenarioInfoList;
    }

    public ScenarioInfo GetScenarioInfo(int id)
    {
        return _scenarioInfoList[id];
    }

    public Dictionary<int, DialogInfo> GetDialogInfoList()
    {
        return _dialogInfoList;
    }

    public DialogInfo GetDialogInfo(int id)
    {
        return _dialogInfoList[id];
    }
}
