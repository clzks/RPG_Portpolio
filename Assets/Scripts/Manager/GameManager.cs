using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public float tick = 0.032f;
    private bool _isPause = false;
    private bool _onePunchMode = false;
    private GameType _gameType = GameType.Count;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            _isPause = !_isPause;

            if (true == _isPause)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            _onePunchMode = !_onePunchMode;
            Debug.Log("�ѹ� ��� :" + _onePunchMode);
        }
    }

    public bool IsOnePunchMode()
    {
        return _onePunchMode;
    }

    public void SetGameType(GameType gameType)
    {
        _gameType = gameType;
    }

    public GameType GetGameType()
    {
        return _gameType;
    }
}
