using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GroundItem : MonoBehaviour, IPoolObject
{
    private ObjectPoolManager _objectPool;
    private GroundItemType _type;
    private int _value;
    private int _id;
    private BaseItem _item;
    [SerializeField]private TextMeshPro _textMesh;
    private BoxCollider _collider;

    private void Awake()
    {
        _objectPool = ObjectPoolManager.Get();
    }

    private void OnEnable()
    {
        _collider.enabled = false;
    }

    public void SetGroundGold(int value)
    {
        _value = value;
    }

    public void SetGroundItem(int id)
    {
        
    }

    public void AnimateItem()
    {
        // 아이템 떨어지는 모션 후 콜라이더 가동
        _collider.enabled = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();

        if (GroundItemType.Item == _type)
        {
            if(true == player.AddItem(_id))
            {
                ReturnObject();
            }
            else
            {
                Debug.Log("아이템을 획득할 수 없습니다");
            }
        }
        else if(GroundItemType.Gold == _type)
        {
            player.AddGold(_value);
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
