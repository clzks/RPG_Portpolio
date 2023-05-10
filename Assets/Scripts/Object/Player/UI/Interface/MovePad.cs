using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class MovePad : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public RectTransform background;
    public RectTransform stick;
    public float moveRadius;
    private Vector3 stickOriginAnchor;
    private Vector3 stickCurrAnchor;
    private Vector3 stickEventPos;
    private bool _isDrag;
    private bool _isFix;
    private Vector3 _stickDir;

    public Image bgImage;
    public Image stickImage;

    private void Awake()
    {
        stickOriginAnchor = stick.position;
        stickCurrAnchor = stickOriginAnchor;
    }

    private void Update()
    {
#if UNITY_EDITOR
        OnKeyboardControl();
#endif
        if ((stickEventPos - stickCurrAnchor).magnitude >= moveRadius)
        {
            stick.position = stickCurrAnchor + _stickDir * moveRadius;
        }
        else
        {
            stick.position = stickEventPos;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(false == _isFix)
        {
            // 패드의 위치를 포인터 위치로 바꾸고 드래그 시작
            stickCurrAnchor = eventData.position;
            background.position = eventData.position;
        }
        else
        {
            stickCurrAnchor = stickOriginAnchor;
        }
        _isDrag = true;
        SetMovePad();
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
            stickEventPos = stickOriginAnchor;
        }
        else
        {
            if(stickEventPos == stickOriginAnchor)
            {
                return;
            }
            _stickDir = (stickEventPos - stickOriginAnchor).normalized;
            _isDrag = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        stickEventPos = eventData.position;
    
        _stickDir = (stickEventPos - stickCurrAnchor).normalized;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        ExecuteResetMovePad();
        SetMovePad();
    }

    public Vector3 GetStickDirection()
    {
        return _stickDir;
    }

    public bool IsDrag()
    {
        return _isDrag;
    }

    public void SetFix(bool isFix)
    {
        _isFix = isFix;
    }

    public void ExecuteResetMovePad()
    {
        stick.position = stickOriginAnchor;
        background.position = stickOriginAnchor;
        stickCurrAnchor = stickOriginAnchor;
        _isDrag = false;
    }

    public void SetMovePad()
    {
        if(true == _isFix)
        {
            bgImage.color = new Color(1, 1, 1, 1);
            stickImage.color = new Color(1, 1, 1, 1);
        }
        else
        {
            if (false == _isDrag)
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
