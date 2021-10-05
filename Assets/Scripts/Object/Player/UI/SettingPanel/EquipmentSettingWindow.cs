using UnityEngine;
using System;
using System.Collections.Generic;

public class EquipmentSettingWindow : MonoBehaviour
{
    public List<EquipSlot> equipSlotList;


    private void Awake()
    {
        
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
}
