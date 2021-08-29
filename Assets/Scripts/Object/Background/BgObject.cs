using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgObject : MonoBehaviour
{
    private ObjectPoolManager _ojbectPool;
    private MeshRenderer _renderer;
    private Material _normalMaterial;
    private Material _transMaterial;

    private void Awake()
    {
        _renderer = GetComponentInChildren<MeshRenderer>();
        _ojbectPool = ObjectPoolManager.Get();
        _normalMaterial = _ojbectPool.GetMaterial("DesertRock_Normal");
        _transMaterial = _ojbectPool.GetMaterial("DesertRock_TransParent");
    }

    public void SetTransParent(bool enabled)
    {
        if(true == enabled)
        {
            _renderer.material = _transMaterial;
        }
        else
        {
            _renderer.material = _normalMaterial;
        }
    }
}
