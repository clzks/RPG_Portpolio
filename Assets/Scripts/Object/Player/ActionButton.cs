using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _isClick = false;
    //private ActionInfo _info;
    private ActionType _type;
    [SerializeField]private string _actionName;

    public void OnPointerDown(PointerEventData eventData)
    {
        // ��Ÿ���̳� �Ҹ� ���� ����ؾ��Ѵ� 
        _isClick = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        _isClick = false;
    }

    public bool GetButtonDown()
    {
        return _isClick;
    }


    public string GetActionName()
    {
        return _actionName;
    }
}
