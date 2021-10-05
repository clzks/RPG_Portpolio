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
        // ������ �������� ��� �� �ݶ��̴� ����
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
                Debug.Log("�������� ȹ���� �� �����ϴ�");
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
