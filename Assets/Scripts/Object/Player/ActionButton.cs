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
        // 쿨타임이나 소모값 등을 계산해야한다 
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
