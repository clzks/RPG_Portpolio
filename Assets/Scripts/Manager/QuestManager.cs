using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    private Dictionary<int, QuestInfo> _questList;
    //private QuestInfo _currQuest = null;
    private bool _isStart = false;

    public void InitQuest(Dictionary<int, QuestInfo> questList)
    {
        _questList = questList;
    }

    public void StartQuest()
    {
        _isStart = true;
    }

    public void ClearQuest()
    {
        _isStart = false;
    }

    public bool IsStart()
    {
        return _isStart;
    }

    public QuestInfo GetQuestInfo(int id)
    {
        if(-1 == id)
        {
            return null;
        }

        return _questList[id];
    }
}
