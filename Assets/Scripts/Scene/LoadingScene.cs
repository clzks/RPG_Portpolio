using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    private DataManager _dataManager;
    private ObjectPoolManager _poolManager;

    private async UniTask Awake()
    {
        _dataManager = DataManager.Get();
        _poolManager = ObjectPoolManager.Get();

        var loadComplete = await _dataManager.LoadPlayerActionList();
        loadComplete &= await _dataManager.LoadEnemyActionList();
        loadComplete &= await _dataManager.LoadEnemyInfoList();
        loadComplete &= await _dataManager.LoadMapInfoList();
        loadComplete &= await _dataManager.LoadBuffInfoList();

        if (true == loadComplete)
        {
            _poolManager.InitPool();
            _poolManager.LoadPrefabs();
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            Debug.Log("읽어들이지 못한 파일이 있습니다");
        }
    }
}
