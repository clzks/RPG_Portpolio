using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour, IPointerClickHandler
{
    public EquipType _type;
    public Image image;
    private int _itemId;
    private UnityAction _action;
    public GameObject infoPanel;


    public void OnPointerClick(PointerEventData eventData)
    {
        _action.Invoke();
    }

    public void SetAction(UnityAction action)
    {
        _action = action;
    }
}

