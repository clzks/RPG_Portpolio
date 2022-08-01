using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DetailedQuestBoard : MonoBehaviour
{
    private ObjectPoolManager _obejctPool;
    private DataManager _dataManager;
    [SerializeField] private Text _client;
    [SerializeField] private Text _description;
    [SerializeField] private Text _subject;
    [SerializeField] private Transform _rewardIconParent;
    private List<RewardIcon> _rewardIcons;

    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _getRewardButton;
    [SerializeField] private Button _acceptQuestButton;
    [SerializeField] private Toggle _toggle;
    private float _toggleTimer;
    private Vector3 _originPos;
    private Vector3 _destPos;
    private Vector3 Position { get { return transform.position; } set { transform.position = value; } }
    private void Awake()
    {
        _obejctPool = ObjectPoolManager.Get();
        _dataManager = DataManager.Get();
        _originPos = Position;
        _destPos = _originPos + new Vector3(810, 0, 0);
        _toggle.onValueChanged.AddListener(delegate { ResetToggleTimer(_toggle); });
        _rewardIcons = new List<RewardIcon>();
    }

    private void Update()
    {
        _toggleTimer += Time.deltaTime * 0.5f;

        if (_toggleTimer >= 1f)
        {
            _toggleTimer = 1f;
        }

        if (true == _toggle.isOn)
        {
            Position = Vector3.Lerp(Position, _destPos, _toggleTimer);
        }
        else
        {
            Position = Vector3.Lerp(Position, _originPos, _toggleTimer);
        }
    }

    public void SetActiveGetRewardButton(bool enabled)
    {
        _getRewardButton.interactable = enabled;
    }

    public void SetDetailedBoard(QuestInfo info)
    {
        _client.text = info.Client;

        // 퀘스트 중인 상태
        _description.text = info.Description;
        SetActiveGetRewardButton(false);
    }

    public void SetReadyDetailedBoard(QuestInfo info)
    {
        _description.text = info.Description;
    }


    public void SetClearDetailedBoard(QuestInfo info)
    {
        if (null != info)
        {
            // 퀘스트를 완수하고 보상만 남은 경우
            _description.text = info.ClearDescription;
            SetActiveGetRewardButton(true);
        }
        else
        {
            UpdateButtons(QuestProcessType.NoneQuest);
            _client.text = "";
            _description.text = "현재 퀘스트가 존재하지 않습니다.";
            _subject.text = "";
        }
    }

    public void SetEmptyDetailedBoard()
    {
        _client.text = "";
        _description.text = "현재 퀘스트가 존재하지 않습니다.";
        _subject.text = "";
    }

    public void UpdateSubjectText(string subjectText)
    {
        _subject.text = subjectText;
    }

    public void ClearRewardIcon(QuestInfo nextQuestInfo)
    {
        foreach (var item in _rewardIcons)
        {
            item.ReturnObject();
        }

        _rewardIcons.Clear();

        if (null != nextQuestInfo)
        {
            // 보상 아이콘 배정
            for (int i = 0; i < nextQuestInfo.Rewards.Count; ++i)
            {
                var reward = nextQuestInfo.Rewards[i];

                var icon = _obejctPool.MakeObject(ObjectType.RewardIcon).GetComponent<RewardIcon>();
                icon.transform.SetParent(_rewardIconParent);
                _rewardIcons.Add(icon);

                switch (reward.Type)
                {
                    case RewardType.Item:
                        icon.SetIcon(_obejctPool.GetSprite("Bag"), reward.TargetValue.ToString(), _dataManager.GetItemInfo(reward.TargetId).Name);
                        break;
                    case RewardType.Exp:
                        icon.SetIcon(_obejctPool.GetSprite("Apple"), reward.TargetValue.ToString(), "Exp");
                        break;
                    case RewardType.Gold:
                        icon.SetIcon(_obejctPool.GetSprite("Gem"), reward.TargetValue.ToString(), "G");
                        break;
                    case RewardType.Stat:

                        break;
                    case RewardType.Count:

                        break;
                }
            }
        }
    }

    public bool IsOn()
    {
        return _toggle.isOn;
    }

    public void SetToggleOn()
    {
        _toggle.isOn = true;
    }

    public void OnClickCancelButton()
    {
        _toggle.isOn = false;
    }

    public void ResetToggleTimer(Toggle toggle)
    {
        _toggleTimer = 0f;
    }

    public void SetAcceptQuestButton(UnityAction action)
    {
        _acceptQuestButton.onClick.AddListener(() => action.Invoke());
    }

    public void SetGetRewardButton(UnityAction action)
    {
        _getRewardButton.onClick.AddListener(() => action.Invoke());
    }

    public void UpdateButtons(QuestProcessType type)
    {
        switch (type)
        {
            case QuestProcessType.NoneQuest:
                if (false == _acceptQuestButton.gameObject.activeSelf)
                {
                    _acceptQuestButton.gameObject.SetActive(false);
                }

                if (true == _getRewardButton.gameObject.activeSelf)
                {
                    _getRewardButton.gameObject.SetActive(false);
                }
                break;

            case QuestProcessType.ReadyToQuest:
                if (false == _acceptQuestButton.gameObject.activeSelf)
                {
                    _acceptQuestButton.gameObject.SetActive(true);
                }

                if(true == _getRewardButton.gameObject.activeSelf)
                {
                    _getRewardButton.gameObject.SetActive(false);
                }
                break;

            case QuestProcessType.Progress:
                if(true == _acceptQuestButton.gameObject.activeSelf)
                {
                    _acceptQuestButton.gameObject.SetActive(false);
                }

                if (false == _getRewardButton.gameObject.activeSelf)
                {
                    _getRewardButton.gameObject.SetActive(true);
                }

                _getRewardButton.interactable = false;
                break;

            case QuestProcessType.Satisfy:
                if (true == _acceptQuestButton.gameObject.activeSelf)
                {
                    _acceptQuestButton.gameObject.SetActive(false);
                }

                if (false == _getRewardButton.gameObject.activeSelf)
                {
                    _getRewardButton.gameObject.SetActive(true);
                }

                _getRewardButton.interactable = true;
                break;
        }
    }
}
