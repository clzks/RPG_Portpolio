using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SkillSettingWindow : MonoBehaviour
{
    [SerializeField]private Player _player;
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
    public Button exitButton;
    private SkillSettingClickIcon _clickIcon;
    [SerializeField] private GameObject _duplicateObject;
    private string _duplicateSkill;
    public List<SkillSettingTargetButton> skillTargetList;
    private void Awake()
    {
        exitButton.onClick.AddListener(() => OnClickExitButton());
    }

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
            icon.SetClickAction(() => SetSkillInfoPanel(_dataManager.GetActionInfo(skill.name), skill.level, data.SkillPoint, icon));
            icon.SetOnDragAction(() => SetDuplicateSkill(icon.GetSkill()));
            icon.SetEndDragAction(() => SetSkillSlot());
            icon.SetIcon(_dataManager.GetSkillImage(skill.name), skill.level, skill.name);
            icon.SetDuplicateObject(_duplicateObject);

            if (0 == i)
            {
                _clickIcon = icon;
                _clickIcon.SetClickBorder(true);
                SetSkillInfoPanel(_dataManager.GetActionInfo(skill.name), skill.level, data.SkillPoint, icon);
            }
        }

        SetTargetButton();
    }

    public void SetSkillInfoPanel(ActionInfo info, int currSkillLevel, int currPoint, SkillSettingClickIcon icon)
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
        cooltimePanel.SetText(info.CoolTime.ToString() + "ÃÊ");
        requiredManaPanel.SetText(info.Cost.ToString());
        descriptionText.text = "";
        if (_clickIcon != icon)
        {
            _clickIcon.SetClickBorder(false);
            _clickIcon = icon;
        }
    }

    public void SetSkillSlot()
    {
        var target = skillTargetList.FindIndex(x => Vector3.Distance(x.transform.position, _duplicateObject.transform.position) <= 120f);

        if(-1 != target)
        {
            _dataManager.SetPlayerSkillSlot(target, _duplicateSkill);
            //skillTargetList[target].SetImage(_dataManager.GetSkillImage(_duplicateSkill));
            SetTargetListImage();
            _player.SetActionList();
        }
    }

    private void SetTargetButton()
    {
        for(int i = 0; i < skillTargetList.Count; ++i)
        {
            var str = _dataManager.GetSkillSlot(i);
            var index = i;
            skillTargetList[i].SetImage(_dataManager.GetSkillImage(str));
            skillTargetList[i].SetAction(() => SetTargetButtonAction(index));
        }
    }

    private void SetTargetListImage()
    {
        for(int i = 0; i < skillTargetList.Count; ++i)
        {
            var skill = _dataManager.GetSkillSlot(i);
            skillTargetList[i].SetImage(_dataManager.GetSkillImage(skill));
        }
    }

    private void SetTargetButtonAction(int index)
    {
        _dataManager.SetPlayerSkillSlot(index, string.Empty);
        //skillTargetList[index].SetImage(null);
        //_player.SetAction(index);
        SetTargetListImage();
        _player.SetActionList();
    }

    private void SetDuplicateSkill(string skill)
    {
        _duplicateSkill = skill;
    }

    private void OnClickExitButton()
    {
        gameObject.SetActive(false);
    }
}
