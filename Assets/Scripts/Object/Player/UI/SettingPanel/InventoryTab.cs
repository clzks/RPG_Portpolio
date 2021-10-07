using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryTab : MonoBehaviour, IPointerClickHandler
{
    public ItemType type;
    public Image selectImage;
    public Image defaultImage;
    private UnityAction _clickEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        _clickEvent.Invoke();
    }

    public void SetClickEvent(UnityAction clickEvent)
    {
        _clickEvent = clickEvent;
    }
}
