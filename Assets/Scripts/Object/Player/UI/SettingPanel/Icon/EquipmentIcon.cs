using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentIcon : InventoryIcon
{
    [SerializeField] private EquipInfoPanel _panel; 
    public override void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void ViewEquipInfoPanel(bool enabled)
    {
        if (true == enabled)
        {
            borderImage.color = new Color(1, 1, 0, 1);
        }
        else
        {
            borderImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }
}
