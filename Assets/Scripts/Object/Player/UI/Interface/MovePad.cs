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

    public Image bgImage;
    public Image stickImage;
    public bool isActive;

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
        if(false == isActive)
        {
            // 패드의 위치를 포인터 위치로 바꾸고 드래그 시작

        }

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
        ExecuteResetMovePad();
    }

    public Vector3 GetStickDirection()
    {
        return _stickDir;
    }

    public bool IsDrag()
    {
        return _isDrag;
    }

    public void ExecuteResetMovePad()
    {
        stick.position = stickOriginPos;
        _isDrag = false;
    }

    public void SetMovePad(bool isFix)
    {
        if(true == isFix)
        {
            bgImage.color = new Color(1, 1, 1, 1);
            stickImage.color = new Color(1, 1, 1, 1);
        }
        else
        {
            if (false == isActive)
            {
                bgImage.color = new Color(1, 1, 1, 0.2f);
                stickImage.color = new Color(1, 1, 1, 0.2f);
            }
            else
            {
                bgImage.color = new Color(1, 1, 1, 1);
                stickImage.color = new Color(1, 1, 1, 1);
            }
        }
    }
}
