using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingWindow : MonoBehaviour
{
    [SerializeField] private Button _exitButton;
    private DataManager _dataManager;
    [SerializeField] Toggle _homingToggle;
    [SerializeField] Toggle _skillDirToggle;
    [SerializeField] Toggle _fixStickToggle;
    public void Init()
    {
        _dataManager = DataManager.Get();

        _homingToggle.isOn = _dataManager.GetGameSettingData().IsHoming;
        _skillDirToggle.isOn = _dataManager.GetGameSettingData().IsSkillDirection;
        _fixStickToggle.isOn = _dataManager.GetGameSettingData().IsFixStick;

        _homingToggle.onValueChanged.AddListener(delegate { OnClickToggle(GameSettingType.Homing, _homingToggle); });
        _fixStickToggle.onValueChanged.AddListener(delegate { OnClickToggle(GameSettingType.FixStick, _fixStickToggle); });
        _skillDirToggle.onValueChanged.AddListener(delegate { OnClickToggle(GameSettingType.SkillDirection, _skillDirToggle); });
        _exitButton.onClick.AddListener(OnClickExitButton);
    }

    public void OnClickToggle(GameSettingType type, Toggle toggle)
    {
        _dataManager.SetGameSettingData(type, toggle.isOn);
        _dataManager.SaveGameSettingData();
    }

    public void OnClickExitButton()
    {
        gameObject.SetActive(false);
    }
}
