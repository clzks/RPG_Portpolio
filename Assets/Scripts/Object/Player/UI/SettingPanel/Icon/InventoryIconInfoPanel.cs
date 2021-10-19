using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryIconInfoPanel : MonoBehaviour
{
    public RectTransform rectTransform;
    public int startHeight;
    public int lineHeight;
    private EquipType _equipType;
    private ItemInfo _info;
    public Text itemName;
    public Text infoText;
    public Text description;
    public Button wearUnwearButton;
    public Text wearUnwearText;
    public bool isWear;
    private UnityAction wearAction;
    private UnityAction unwearAction;
    
    private void Awake()
    {
        wearUnwearButton.onClick.AddListener(() => OnClickWearUnwearButton());
    }

    public void SetPanel(ItemInfo info, bool wear)
    {
        itemName.text = info.DisplayName;
        _info = info;
        var status = info.Values;
        string str = "";

        int count = 0;

        switch (info.Type)
        {
            case ItemType.Weapon:
            case ItemType.Armor:
            case ItemType.Accessory:
                _equipType = Formula.ConvertItemTypeToEquipType(info.Type);

                if (status.MaxHp != 0)
                {
                    str += "�ִ� ü�� : " + status.MaxHp + "\n";
                    count += 1;
                }

                if (status.Speed != 0)
                {
                    str += "�̵��ӵ� : " + status.Speed + "\n";
                    count += 1;
                }

                if (status.AttackSpeed != 0)
                {
                    str += "���ݼӵ� : " + status.AttackSpeed + "\n";
                    count += 1;
                }

                if (status.Damage != 0)
                {
                    str += "���ݷ� : " + status.Damage + "\n";
                    count += 1;
                }

                if(status.Defence != 0)
                {
                    str += "���� : " + status.Defence;
                    count += 1;
                }

                infoText.text = str;
                break;
            case ItemType.Quest:
                infoText.text = "";
                break;
            case ItemType.Consumable:
                infoText.text = "";
                break;
        }
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, startHeight + count * lineHeight);
        description.text = info.Description;
        isWear = wear;
        if(true == isWear)
        {
            wearUnwearText.text = "����";
        }
        else
        {
            wearUnwearText.text = "����";
        }
    }

    public void OnClickWearUnwearButton()
    {
        if(true == isWear)
        {
            wearAction.Invoke();
        }
        else
        {
            unwearAction.Invoke();
        }

        gameObject.SetActive(false);
    }

    public void SetWearAction(UnityAction action)
    {
        wearAction = action;
    }

    public void SetUnwearAction(UnityAction action)
    {
        unwearAction = action;
    }

    public EquipType GetEquipType()
    {
        return _equipType;
    }
    
    public ItemInfo GetItemInfo()
    {
        return _info;
    }

    private void CloseInfoPanel()
    {
        gameObject.SetActive(false);
    }
}
    