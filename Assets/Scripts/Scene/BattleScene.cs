using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScene : MonoBehaviour
{
    private DataManager _dataManager;
    private ObjectPoolManager _objectPool;
    private BaseMap _currMap = null;
    private Player _player;
    public MiniMap _miniMap;

    private void Awake()
    {
        _dataManager = DataManager.Get();
        _objectPool = ObjectPoolManager.Get();
        _player = GameObject.Find("Player").GetComponent<Player>();
        EnterNewWorld(10010, 0);
    }
    
    public void EnterNewWorld(int worldId, int SummonIndex)
    {
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
}
