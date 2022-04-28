using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GroundItem : MonoBehaviour, IPoolObject
{
    private ObjectPoolManager _objectPool;
    private DataManager _dataManager;
    private GroundItemType _type;
    private int _value;
    private int _id;
    private BaseItem _item;
    [SerializeField]private TextMeshPro _textMesh;
    [SerializeField]private BoxCollider _collider;

    private void Awake()
    {
        _dataManager = DataManager.Get();
        _objectPool = ObjectPoolManager.Get();
    }

    private void OnEnable()
    {
        _collider.enabled = false;
        StartCoroutine(AnimateItem());
    }

    public void SetGroundGold(int value)
    {
        _type = GroundItemType.Gold;
        _value = value;
        _textMesh.text = value.ToString() + "∞ÒµÂ";
    }

    public void SetGroundItem(int id)
    {
        _type = GroundItemType.Item;
        var info = _dataManager.GetItemInfo(id);
        _id = id;
        _textMesh.text = info.DisplayName;
    }

    public IEnumerator AnimateItem()
    {
        // æ∆¿Ã≈€ ∂≥æÓ¡ˆ¥¬ ∏º« »ƒ ƒ›∂Û¿Ã¥ı ∞°µø
        var timer = 0f;

        while(timer <= .3f)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        _collider.enabled = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();

        if (GroundItemType.Item == _type)
        {
            if(true == player.AddItem(_id))
            {
                Debug.Log("æ∆¿Ã≈€¿ª »πµÊ!");
                ReturnObject();
            }
            else
            {
                Debug.Log("æ∆¿Ã≈€¿ª »πµÊ«“ ºˆ æ¯Ω¿¥œ¥Ÿ");
            }
        }
        else if(GroundItemType.Gold == _type)
        {
            Debug.Log("∞ÒµÂ »πµÊ!");
            player.AddGold(_value);
            ReturnObject();
        }
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
        return name;
    }

    public ObjectType GetObjectType()
    {
        return ObjectType.GroundItem;
    }

    public void ReturnObject()
    {
        _textMesh.text = "";
        _objectPool.ReturnObject(this);
    }
}
