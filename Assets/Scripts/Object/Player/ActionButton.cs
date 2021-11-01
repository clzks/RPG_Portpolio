using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _isClick = false;
    //private ActionInfo _info;
    private ActionType _type;
    [SerializeField] private string _actionName;
    [SerializeField] private Image _skillImage;
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
