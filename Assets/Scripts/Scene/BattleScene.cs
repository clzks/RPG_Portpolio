using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScene : MonoBehaviour
{
    private ObjectPoolManager _objectPool;
    private BaseMap _currMap = null;
    private Player _player;

    private void Awake()
    {
        _objectPool = ObjectPoolManager.Get();
        _player = GameObject.Find("Player").GetComponent<Player>();
        EnterNewWorld(10010);
    }
    
    public void EnterNewWorld(int worldId)
    {
        _player.SetActiveNavMeshAgent(false);

        if (null != _currMap)
        {
            _currMap.ReturnObject();
        }

        _currMap = _objectPool.MakeObject(worldId, ObjectType.Map).GetComponent<BaseMap>();
        _currMap.SetPlayer(_player);
        _currMap.Init();
        _player.transform.position = _currMap.GetStartPosition();
        _player.SetActiveNavMeshAgent(true);
    }

}
