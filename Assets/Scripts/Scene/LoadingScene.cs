using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    private async UniTask Awake()
    {
        var loadComplete = await DataManager.Get().LoadPlayerActionList();

        if (true == loadComplete)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
