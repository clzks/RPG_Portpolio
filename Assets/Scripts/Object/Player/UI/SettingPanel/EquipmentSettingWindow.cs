using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSettingWindow : MonoBehaviour, IPointerClickHandler
{
    private DataManager _dataManager;
    private ObjectPoolManager _objectPool;
    public Button exitButton;
    public List<EquipmentIcon> equipIconList;
    public Transform inventorySlotParent;
    public List<InventoryTab> _tabList;
    public ItemType currClickTabType = ItemType.Weapon;
    [SerializeField] private Player _player;
    private List<InventoryIcon> _iconList;
    private InventoryIcon _currSelectInventoryIcon;
    
    private void Awake()
    {
        _dataManager = DataManager.Get();
        _objectPool = ObjectPoolManager.Get();
        _iconList = new List<InventoryIcon>();
        SetInventoryTabClickEvent();
        SetEquipSlot();
        exitButton.onClick.AddListener(() => OnClickExitButton());
    }

    private void OnEnable()
    {
        UpdateInventoryIcon();
    }

    private void OnDisable()
    {
        currClickTabType = ItemType.Weapon;
    }

    public void SetEquipSlot()
    {
        var equimpmentList = _player.GetEquipmentList();
        ItemInfo info = null;
        foreach (var item in equipIconList)
        {
            item.SetClickEvents(() => OnClickIconObject(item));
            item.AddListenerToUnwearButton(() => OnClickUnwearButton(item.itemType));
            switch (item.itemType)
            {
                case EquipType.Weapon:
                    info = _dataManager.GetItemInfo(equimpmentList[0]);
                    break;
                case EquipType.Armor:
                    info = _dataManager.GetItemInfo(equimpmentList[1]);
                    break;
                case EquipType.Accessory:
                    info = _dataManager.GetItemInfo(equimpmentList[2]);
                    break;
            }

            if (null != info)
            {
                item.SetImage(_objectPool.GetSprite(info.ImageName));
                item.SetId(info.Id);
            }
            else
            {
                item.SetEmptyImage();
                item.SetId(-1);
            }
        }
    }

    public void OnClickIconObject(InventoryIcon obj)
    {
        OnClickIcon(obj);
        _currSelectInventoryIcon = obj;
    }

    public void OnClickInventoryTab(ItemType type)
    {
        if (currClickTabType != type)
        {
            currClickTabType = type;

            UpdateInventoryIcon();
        }
    }

    public void SetClickEquipmentIcon()
    {

    }

    public void UpdateInventoryIcon()
    {
        if(_iconList.Count != 0)
        {
            for(int i = _iconList.Count - 1; i >= 0; --i)
            {
                _iconList[i].ReturnObject();
            }

            _iconList.Clear();
        }

        var inventory = _player.GetInventory();
        var itemList = inventory[currClickTabType];
        foreach (var item in itemList)
        {
            var info = _dataManager.GetItemInfo(item.Key);

            if (info.Type == ItemType.Consumable || info.Type == ItemType.Quest)
            {
                MakeInventoryIcon(info, true, item.Value);
            }
            else
            {
                for (int i = 0; i < item.Value; ++i)
                {
                    MakeInventoryIcon(info, false);
                }
            }
        }
    }

    private void MakeInventoryIcon(ItemInfo info, bool isConsumption, int count = 1)
    {
        var icon = _objectPool.MakeObject(ObjectType.InventoryIcon).GetComponent<InventoryIcon>();
        icon.SetImage(_objectPool.GetSprite(info.ImageName));
        if(true == isConsumption)
        {
            icon.SetCountText(count);
        }
        else
        {
            icon.ResetCountText();
        }

        icon.transform.SetParent(inventorySlotParent);
        icon.SetClickEvents(() => OnClickIconObject(icon));
        _iconList.Add(icon);
    }


    public void SetInventoryTabClickEvent()
    {
        foreach (var item in _tabList)
        {
            item.SetClickEvent(() => OnClickInventoryTab(item.type));
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (_currSelectInventoryIcon != null)
        {
            _currSelectInventoryIcon.ViewIconDescription(false);
            _currSelectInventoryIcon = null;
        }
    }

    private void OnClickIcon(InventoryIcon obj)
    {
        if (_currSelectInventoryIcon != null)
        {
            if (obj != _currSelectInventoryIcon.GetObject())
            {
                _currSelectInventoryIcon.ViewIconDescription(false);
                _currSelectInventoryIcon = null;
            }
        }
    }

    private void OnClickUnwearButton(EquipType type)
    {
        var equipList = _player.GetEquipmentList();

        switch (type)
        {
            case EquipType.Weapon:
                if (_player.AddItem(equipList[0]))
                {
                    equipList[0] = -1;
                    equipIconList[0].SetEmptyImage();
                }
                break;
            case EquipType.Armor:
                if (_player.AddItem(equipList[1]))
                {
                    equipList[1] = -1;
                    equipIconList[1].SetEmptyImage();
                }
                break;
            case EquipType.Accessory:
                if (_player.AddItem(equipList[2]))
                {
                    equipList[2] = -1;
                    equipIconList[2].SetEmptyImage();
                }
                break;
        }

        UpdateInventoryIcon();
    }

    private void OnClickExitButton()
    {
        gameObject.SetActive(false);
    }
}
