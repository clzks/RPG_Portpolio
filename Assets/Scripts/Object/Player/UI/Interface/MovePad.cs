using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MovePad : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public RectTransform background;
    public RectTransform stick;
    public float moveRadius;
    private Vector3 stickOriginPos;
    private bool _isDrag;
    private Vector3 _stickDir;

    private void Awake()
    {
        stickOriginPos = stick.position;
    }

    private void Update()
    {
#if UNITY_EDITOR
        OnKeyboardControl();
#endif
        if ((stick.position - stickOriginPos).magnitude >= moveRadius)
        {
            stick.position = stickOriginPos + _stickDir * moveRadius;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDrag = true;
    }

    public void OnKeyboardControl()
    {
        if (Input.GetKey(KeyCode.W))
        {
            stick.position += new Vector3(0, moveRadius, 0);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            stick.position += new Vector3(0, -moveRadius, 0);
        }

        if (Input.GetKey(KeyCode.A))
        {
            stick.position += new Vector3(-moveRadius, 0, 0);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            stick.position += new Vector3(moveRadius, 0, 0);
        }
        
        if (false == Input.anyKey)
        {
            _isDrag = false;
            stick.position = stickOriginPos;
        }
        else
        {
            if(stick.position == stickOriginPos)
            {
                return;
            }
            _stickDir = (stick.position - stickOriginPos).normalized;
            _isDrag = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        stick.position = eventData.position;

        _stickDir = (stick.position - stickOriginPos).normalized;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        stick.position = stickOriginPos;
        _isDrag = false;
    }

    public Vector3 GetStickDirection()
    {
        return _stickDir;
    }

    public bool IsDrag()
    {
        return _isDrag;
    }
}
