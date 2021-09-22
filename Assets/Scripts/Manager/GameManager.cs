using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public float tick = 0.032f;
    private bool _isPause = false;


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
    }
}
