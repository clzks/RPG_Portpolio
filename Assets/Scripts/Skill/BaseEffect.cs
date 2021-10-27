using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect : MonoBehaviour, IPoolObject
{
    private int _id;
    private string _name;
    private ObjectPoolManager _objectPool;
    private GameObject _targetObject = null;
    private Vector3 _targetPos;
    private void Awake()
    {
        _objectPool = ObjectPoolManager.Get();
    }

    public void ExecuteCountDown(float life)
    {
        StartCoroutine(CountDown(life));
    }

    private IEnumerator CountDown(float life)
    {
        yield return new WaitForSeconds(life);

        ReturnObject();
    }

    private void Update()
    {
        if(null != _targetObject)
        {
            transform.position = _targetObject.transform.position + _targetPos;
        }
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetRotateAround(Vector3 point, Vector3 axis, float angle)
    {
        transform.RotateAround(point, axis, angle);
    }

    public void SetParent(Transform tr)
    {
        transform.SetParent(tr);
    }

    public string GetName()
    {
        return _name;
    }

    public void SetName(string name)
    {
        _name = name;
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
        transform.rotation = new Quaternion();
        _objectPool.ReturnObject(this);
    }

    public void SetTargetObject(GameObject obj)
    {
        _targetObject = obj;
    }

    public void SetTargetPos(Vector3 targetPos)
    {
        _targetPos = targetPos;
    }
}
