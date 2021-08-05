using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    private DataManager _dataManager;
    private async UniTask Awake()
    {
        _dataManager = DataManager.Get();
        var loadComplete = await _dataManager.LoadPlayerActionList();
        loadComplete &= await _dataManager.LoadEnemyActionList();

        if (true == loadComplete)
        {
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            Debug.Log("읽어들이지 못한 파일이 있습니다");
        }
    }
}
