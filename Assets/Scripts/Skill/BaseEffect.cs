using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect : MonoBehaviour, IPoolObject
{
    private int _id;
    private string _name;
    private ObjectPoolManager _objectPool;

    private void Awake()
    {
        _objectPool = ObjectPoolManager.Get();
    }

    public void SetParent(Transform tr)
    {
        transform.SetParent(tr);
    }

    public string GetName()
    {
        return _name;
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public ObjectType GetObjectType()
    {
        return ObjectType.Effect;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void ReturnObject()
    {
        _objectPool.ReturnObject(this);
    }
}
