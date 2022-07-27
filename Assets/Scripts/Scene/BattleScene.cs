using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattleScene : MonoBehaviour
{
    private GameManager _gameManager;
    private DataManager _dataManager;
    private ObjectPoolManager _objectPool;
    private ScenarioManager _scenarioManager;
    private BaseMap _currMap = null;
    private Player _player;
    public MiniMap _miniMap;
    [SerializeField] private GameObject _equipmentWindowPopUp;
    [SerializeField] private SkillSettingWindow _skillWindowPopUp;
    [SerializeField] private GameObject SettingWindowPopUp;

    private void Awake()
    {
        _gameManager = GameManager.Get();
        _scenarioManager = ScenarioManager.Get();
        _dataManager = DataManager.Get();
        _objectPool = ObjectPoolManager.Get();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _skillWindowPopUp.Init(_dataManager.GetPlayerData());
    }

    private void Start()
    {
        EnterNewWorld(_player.GetMapId(), -1);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            EnterNewWorld(10020, 4);
        }
    }

    public void EnterNewWorld(int worldId, int SummonIndex)
    {
        _objectPool.ReturnAllObject();
        _player.SetActiveNavMeshAgent(false);

        if (null != _currMap)
        {
            _currMap.ReturnObject();
        }

        // 게임을 처음 실행했을 경우
        if(-1 == worldId)
        {
            worldId = 10010;
            SummonIndex = 0;
            _currMap = _objectPool.MakeObject(ObjectType.Map, worldId).GetComponent<BaseMap>();
            _currMap.SetMap(_dataManager.GetMapInfo(worldId));
            _currMap.SetPlayer(_player);
            _currMap.Init();
            _player.transform.position = _currMap.GetPointPosition(SummonIndex);
            _player.SetActiveNavMeshAgent(true);
            _player.SetMapId(worldId);
        }
        // 게임을 실행한적이 있는 경우
        else
        {
            _currMap = _objectPool.MakeObject(ObjectType.Map, worldId).GetComponent<BaseMap>();
            _currMap.SetMap(_dataManager.GetMapInfo(worldId));
            _currMap.SetPlayer(_player);
            _currMap.Init();
            _player.SetActiveNavMeshAgent(true);
            _player.SetMapId(worldId);
            // 게임을 불러온 경우
            if (-1 == SummonIndex)
            {
                _player.transform.position = _player.GetSavedPosition();
            }
            // 맵 포인트를 통해서 이동한 경우
            else
            {
                _player.transform.position = _currMap.GetPointPosition(SummonIndex);
            }
        }

        _player.SetMapPointList(_currMap.GetMapPointList());
    }

    public void OnClickSave()
    {
        _player.SetPostionData();               
        DataManager.Get().SavePlayerData();     
    }

    public void OnClickEquipmentButton()
    {
        _equipmentWindowPopUp.SetActive(true);
    }

    public void onClickSkillSettingButton()
    {
        _skillWindowPopUp.gameObject.SetActive(true);
    }

    public void OnClickSkillButton()
    {

    }

    public void OnClickSettingButton()
    {

    }

    public void PlayerEnterMapPoint(MapPoint mapPoint)
    {
        _player.IsQuestPointArrive(mapPoint);
    }
}
