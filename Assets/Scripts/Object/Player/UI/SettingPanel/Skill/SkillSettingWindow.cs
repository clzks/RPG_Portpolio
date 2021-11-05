using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSettingWindow : MonoBehaviour
{
    private ObjectPoolManager _objectPool;
    private DataManager _dataManager;
    [SerializeField]private Transform contentPanel;
    public Text nameText;
    public TextValuePanel currSkillLevelPanel;
    public TextValuePanel currSkillPointPanel;
    public TextValuePanel requiredLevelPanel;
    public TextValuePanel cooltimePanel;
    public TextValuePanel requiredManaPanel;
    public Text descriptionText;

    public void Init(PlayerData data)
    {
        _objectPool = ObjectPoolManager.Get();
        _dataManager = DataManager.Get();

        var list = data.SkillList;
        for(int i = 0; i < list.Count; ++i)
        {
            SkillInfo skill = list[i];
            var obj = Instantiate(_objectPool.GetObject("SkillSettingClickIcon"));
            obj.transform.SetParent(contentPanel);
            var icon = obj.GetComponent<SkillSettingClickIcon>();
            icon.SetClickAction(() => SetSkillInfo(_dataManager.GetActionInfo(skill.name), skill.level, data.SkillPoint));
        }
    }

    public void SetSkillInfo(ActionInfo info, int currSkillLevel, int currPoint)
    {
        nameText.text = info.Name;
        currSkillLevelPanel.SetText(currSkillLevel.ToString());
        currSkillPointPanel.SetText(currPoint.ToString());
        int requiredLevel = info.CalculateNextLevel(currSkillLevel);
        string requiredLevelText;
        if(-1 == requiredLevel)
        {
            requiredLevelText = "-";
        }
        else
        {
            requiredLevelText = requiredLevel.ToString();
        }
        requiredLevelPanel.SetText(requiredLevelText);
        cooltimePanel.SetText(info.CoolTime.ToString() + "√ ");
        requiredManaPanel.SetText(info.Cost.ToString());
        descriptionText.text = "";
    }
}
