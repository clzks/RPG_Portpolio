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
    //[SerializeField] private Button _unwearItemButton;

    private void Awake()
    {
        // EquipmentIcon�� InvetoryIcon�� ���������, ������ƮǮ���� �ʿ� ����
        // Awake�� ��ӹ��� �ʱ����� �� Awake ���
    }

    private void OnEnable()
    {
        //ActiveUnwearButton(false);
        ViewIconDescription(false);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        OnPointerClickAction();
        if(null == _info)
        {
            return;
        }
        ViewIconDescription(true);
    }

    public override void ViewIconDescription(bool enabled)
    {
        if (true == enabled)
        {
            _infoPanel.SetPanel(transform.position.x, _info, false);
            _infoPanel.gameObject.SetActive(true);
        }
        else
        {
            _infoPanel.gameObject.SetActive(false);
        }
    }

    public void SetId(int id)
    {
        _itemId = id;
    }

    public void SetEmptyImage()
    {
        _info = null;
        itemImage.sprite = _emptySprite;
    }

    //private void ActiveUnwearButton(bool enabled)
    //{
    //    _unwearItemButton.gameObject.SetActive(enabled);
    //}

    //public void AddListenerToUnwearButton(UnityAction action)
    //{
    //    _unwearItemButton.onClick.AddListener(action);
    //    _unwearItemButton.onClick.AddListener(() => ActiveUnwearButton(false));
    //}
}
