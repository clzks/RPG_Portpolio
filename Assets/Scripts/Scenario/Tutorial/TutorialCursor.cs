using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCursor : MonoBehaviour
{
    private Vector3 originPos;
    private Vector3 destPos;
    private float _time = 0.5f;
    private float _timer = 0f;
    [SerializeField]private RectTransform rtTransform;
    public void SetCursor(Vector3 position, bool isReverse = false)
    {
        _timer = 0f;
        rtTransform.position = position;
        originPos = position;
       
        if (true == isReverse)
        {
            rtTransform.localScale = new Vector3(1, -1, 1);
            destPos = originPos - new Vector3(0, -200, 0);
        }
        else
        {
            rtTransform.localScale = new Vector3(1, 1, 1);
            destPos = originPos - new Vector3(0, 200, 0);
        }
    }

    public void Update()
    {
        _timer += Time.deltaTime;
        
        if(_timer >= _time)
        {
            _timer = -0.2f;
        }
        
        float r = _timer * (1 - _time);
        
        if( r < 0)
        {
            r = 0f;
        }
        
        transform.position = Vector3.Lerp(originPos, destPos, r);
    }
}
