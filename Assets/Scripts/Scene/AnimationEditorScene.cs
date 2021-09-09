using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEditorScene : MonoBehaviour
{
    private ObjectPoolManager _poolManager;
    private void Awake()
    {
        _poolManager = ObjectPoolManager.Get();
        _poolManager.InitPool();
        _poolManager.LoadPrefabs();
    }
}
