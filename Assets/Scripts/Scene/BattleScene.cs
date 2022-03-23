using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattleScene : MonoBehaviour
{
    private GameManager _gameManager;
    private DataManager _dataManager;
    private ObjectPoolManager _objectPool;
    private BaseMap _currMap = null;
    private Player _player;
    public MiniMap _miniMap;
    [SerializeField] private GameObject _equipmentWindowPopUp;
    [SerializeField] private SkillSettingWindow _skillWindowPopUp;
    [SerializeField] private GameObject SettingWindowPopUp;

    private void Awake()
    {
        _gameManager = GameManager.Get();
        
        _dataManager = DataManager.Get();
        _objectPool = ObjectPoolManager.Get();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _skillWindowPopUp.Init(_dataManager.GetPlayerData());
        EnterNewWorld(10010, 0);
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

        _currMap = _objectPool.MakeObject(ObjectType.Map, worldId).GetComponent<BaseMap>();
        _currMap.SetMap(_dataManager.GetMapInfo(worldId));
        _currMap.SetPlayer(_player);
        _currMap.Init();
        _player.transform.position = _currMap.GetPointPosition(SummonIndex);
        _player.SetActiveNavMeshAgent(true);
    }

    public void OnClickSave()
    {
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
}
