using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private Dictionary<string, ActionInfo> actionInfoList;

    private void Awake()
    {
        actionInfoList = new Dictionary<string, ActionInfo>();
    }

    public async UniTask<bool> LoadPlayerActionList()
    {
        actionInfoList = await JsonConverter<ActionInfo>.GetJsonToDictionaryKeyName();
        if(null != actionInfoList)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Dictionary<string, ActionInfo> GetActionInfoList()
    {
        return actionInfoList;
    }
}
