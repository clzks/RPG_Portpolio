using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScene : MonoBehaviour
{
    private ObjectPoolManager _poolManager;
    private BaseMap _currMap = null;

    private void Awake()
    {
        EnterNewWorld("FirstWorld");
    }
    
    public void EnterNewWorld(string worldName)
    {
        if(null != _currMap)
        {
            _currMap.ReturnObject();
        }

        _currMap = _poolManager.MakeObject(worldName).GetComponent<BaseMap>();
        _currMap.Init();
    }

}
