using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private Dictionary<string, ActionInfo> actionInfoList;
    private Dictionary<string, Dictionary<string,EnemyAction>> enemyActionList;
    private void Awake()
    {
        actionInfoList = new Dictionary<string, ActionInfo>();
        enemyActionList = new Dictionary<string, Dictionary<string, EnemyAction>>();
    }

    public async UniTask<bool> LoadPlayerActionList()
    {
        actionInfoList = await JsonConverter<ActionInfo>.GetJsonToDictionaryKeyName();
        if(null != actionInfoList)
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
            if(false == enemyActionList.ContainsKey(item.CharacterName))
            {
                Dictionary<string, EnemyAction> actionList = new Dictionary<string, EnemyAction>();
                enemyActionList.Add(item.CharacterName, actionList);
            }

            enemyActionList[item.CharacterName].Add(item.Name, item);
        }
        
        if(null != enemyActionList)
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

    public Dictionary<string, ActionInfo> GetActionInfoList()
    {
        return actionInfoList;
    }

    public EnemyAction GetEnemyActionInfo(string enemyName, string actionName)
    {
        return enemyActionList[enemyName][actionName];
    }
}
