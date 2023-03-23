using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    private GameManager _gameManager;
    private DataManager _dataManager;
    private ObjectPoolManager _objectPool;
    private async UniTask Awake()
    {
        _gameManager = GameManager.Get();
        _dataManager = DataManager.Get();
        _objectPool = ObjectPoolManager.Get();

        var loadComplete = await _dataManager.LoadPlayerActionList();
        loadComplete &= await _dataManager.LoadEnemyActionList();
        loadComplete &= await _dataManager.LoadEnemyInfoList();
        loadComplete &= await _dataManager.LoadMapInfoList();
        loadComplete &= await _dataManager.LoadBuffInfoList();
        loadComplete &= await _dataManager.LoadItemList();
        loadComplete &= await _dataManager.LoadEffectList();
        loadComplete &= _dataManager.LoadSkillImageList();
        loadComplete &= await _dataManager.LoadQuestInfoList();
        loadComplete &= await _dataManager.LoadScenarioInfoList();
        loadComplete &= await _dataManager.LoadDialogInfoList();
        loadComplete &= await _dataManager.LoadOriginPlayerData();
        await _dataManager.LoadPlayerData();
        if (true == loadComplete)
        {
            _objectPool.InitPool();
            _objectPool.LoadPrefabs();
            _objectPool.LoadSprite();
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            Debug.Log("읽어들이지 못한 파일이 있습니다");
        }
    }
}
