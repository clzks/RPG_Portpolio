using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class MainScene : MonoBehaviour
{
    private GameManager _gameManager;
    private DataManager _dataManager;
    public GameObject newGamePopUp;
    public Button loadGameButton;

    private async UniTask Awake()
    {
        _gameManager = GameManager.Get();
        _dataManager = DataManager.Get();
        await _dataManager.LoadPlayerData();
    }

    private void Start()
    {
        if(-1 == _dataManager.GetPlayerData().CurrMapId)
        {
            loadGameButton.interactable = false;
        }
    }

    public void OnClickPlayButton()
    {
        _gameManager.SetGameType(GameType.LoadGame);
        SceneManager.LoadScene("BattleScene");
    }

    public void OnClickEditButton()
    {
        SceneManager.LoadScene("AnimationEditorScene");
    }

    public void OnClickNewGameButton()
    {
        _gameManager.SetGameType(GameType.NewGame);
        _dataManager.MakeNewPlayerData();
        SceneManager.LoadScene("BattleScene");
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}
