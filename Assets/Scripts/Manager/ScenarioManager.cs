using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioManager : Singleton<ScenarioManager>
{
    private Dictionary<int, ScenarioInfo> _scenarioList;
    private Dictionary<int, QuestInfo> _questList;
    //private QuestInfo _currQuest = null;
    //private bool _isStart = false;
    private ScenarioProcessType _type = ScenarioProcessType.Count;

    public void InitScenario(Dictionary<int, ScenarioInfo> scenarioList)
    {
        _scenarioList = scenarioList;
    }

    public void InitQuest(Dictionary<int, QuestInfo> questList)
    {
        _questList = questList;
    }

    public void SetScenarioProcess(ScenarioProcessType type)
    {
        _type = type;
    }

    public ScenarioProcessType GetProcess()
    {
        return _type;
    }

    public QuestInfo GetQuestInfo(int scenarioId)
    {
        if(-1 == scenarioId)
        {
            return null;
        }

        if(false == _scenarioList.ContainsKey(scenarioId))
        {
            return null;
        }

        int questId = _scenarioList[scenarioId].QuestId;

        if (-1 == questId)
        {
            return null;
        }

        return _questList[questId];
    }

    public int GetNextScenarioId(int scenarioId)
    {
        return _scenarioList[scenarioId].NextScenarioId;
    }

    public int GetDialogId(int scenarioId, DialogType type)
    {
        if (type == DialogType.PrevDialog)
        {
            return _scenarioList[scenarioId].ReadyDialogInfoId;
        }
        else if (type == DialogType.ClearDialog)
        {
            return _scenarioList[scenarioId].ClearDialogInfoId;
        }
        else
        {
            return -1;
        }
    }
}
