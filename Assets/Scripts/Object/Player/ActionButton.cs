using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ActionButton : MonoBehaviour, IPointerDownHandler
{
    private bool _isClick = false;
    private ActionInfo _info;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isClick = true;
    }

    private void Update()
    {
        _isClick = false;
    }

    public bool GetButtonDown()
    {
        return _isClick;
    }
}
