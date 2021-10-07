using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class EquipmentSettingWindow : MonoBehaviour
{
    public List<EquipSlot> equipSlotList;
    public Transform inventorySlotParent;
    public List<InventoryTab> _tabList;
    public ItemType currClickTabType = ItemType.Weapon;
    private void Awake()
    {
        SetInventoryTabClickEvent();
    }

    public void SetEquipSlot(EquipSlot slot)
    {
        foreach (var item in equipSlotList)
        {
            //item.SetAction();
        }
    }

    public void OnClickEquipSlot()
    {

    }

    public void SetCurrClickTabType(ItemType type)
    {
        currClickTabType = type;
    }

    public void SetInventoryTabClickEvent()
    {
        foreach (var item in _tabList)
        {
            item.SetClickEvent(() => SetCurrClickTabType(item.type));
        }
    }
}
