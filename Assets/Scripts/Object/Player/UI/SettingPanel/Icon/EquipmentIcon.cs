using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentIcon : InventoryIcon
{
    public EquipType itemType;
    private int _itemId;
    [SerializeField] Sprite _emptySprite;
    [SerializeField] private EquipInfoPanel _panel;
    [SerializeField] private Button _unwearItemButton;

    private void OnEnable()
    {
        ActiveUnwearButton(false);
        ViewIconDescription(false);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        OnPointerClickAction();
        if(-1 == _itemId)
        {
            return;
        }
        ViewIconDescription(true);
    }

    public override void ViewIconDescription(bool enabled)
    {
        ActiveUnwearButton(enabled);
        // 패널에 세팅하는거

        //if (true == enabled)
        //{
        //    borderImage.color = new Color(1, 1, 0, 1);
        //}
        //else
        //{
        //    borderImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        //}
    }

    public void SetId(int id)
    {
        _itemId = id;
    }

    public void SetEmptyImage()
    {
        _itemId = -1;
        itemImage.sprite = _emptySprite;
    }

    private void ActiveUnwearButton(bool enabled)
    {
        _unwearItemButton.gameObject.SetActive(enabled);
    }

    public void AddListenerToUnwearButton(UnityAction action)
    {
        _unwearItemButton.onClick.AddListener(action);
        _unwearItemButton.onClick.AddListener(() => ActiveUnwearButton(false));
    }
}
