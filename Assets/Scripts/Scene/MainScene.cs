using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainScene : MonoBehaviour
{
    public Button playButton;
    public Button editButton;

    public void OnClickPlayButton()
    {
        SceneManager.LoadScene("BattleScene");
    }

    public void OnClickEditButton()
    {
        SceneManager.LoadScene("AnimationEditorScene");
    }
}
