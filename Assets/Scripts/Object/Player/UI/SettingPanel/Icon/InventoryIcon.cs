using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryIcon : MonoBehaviour, IPoolObject, IPointerClickHandler
{
    protected ObjectPoolManager _obejctPool;
    public Image itemImage;
    public Image borderImage;
    public Text countText;
    protected UnityAction _clickEvent;

    private void Awake()
    {
        _obejctPool = ObjectPoolManager.Get();
    }

    public void SetImage(Sprite image)
    {
        itemImage.sprite = image;
    }


    public void SetCountText(int count)
    {
        countText.text = "x" + count.ToString();
    }

    public void ResetCountText()
    {
        countText.text = "";
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public string GetName()
    {
        return "";
    }

    public ObjectType GetObjectType()
    {
        return ObjectType.InventoryIcon;
    }

    public void ReturnObject()
    {
        _obejctPool.ReturnObject(this);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        OnPointerClickAction();
        ViewIconDescription(true);
    }

    public virtual void ViewIconDescription(bool enabled)
    {
        if(true == enabled)
        {
            borderImage.color = new Color(1, 1, 0, 1);
        }
        else
        {
            borderImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }

    public void SetClickEvents(UnityAction clickEvent)
    {
        _clickEvent = clickEvent;
    }

    protected void OnPointerClickAction()
    {
        _clickEvent.Invoke();
    }
}
