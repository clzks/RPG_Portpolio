using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect : MonoBehaviour, IPoolObject
{
    protected ObjectPoolManager _objectPool;
    protected DataManager _dataManager;
    protected int _id;
    protected string _name;
    protected GameObject _targetObject = null;
    protected Vector3 _targetPos;
    protected ActionInfo _action;
    protected IActor _actor;
    private List<HitUnitInfo> HitUnitList { get { return _action.HitUnitList; } }
    private void Awake()
    {
        _dataManager = DataManager.Get();
        _objectPool = ObjectPoolManager.Get();
    }

    public void ExecuteEffect(float life)
    {
        if (null != _actor)
        {
            StartCoroutine(ExecuteHitUnit(life));
        }
        else
        {
            StartCoroutine(Execute(life));
        }
    }

    private IEnumerator ExecuteHitUnit(float life)
    {
        float timer = 0f;
        int hitUnitCount = HitUnitList.Count;
        int index = 0;

        while (timer <= life)
        {
            if(index < hitUnitCount && timer >= HitUnitList[index].StartTimer)
            {
                HitUnit hitUnit = _objectPool.MakeObject(ObjectType.HitUnit, "NormalHitUnit").GetComponent<HitUnit>();
                HitUnitInfo info = HitUnitList[index];
                hitUnit.SetHitUnit(_actor, _action.DuplicatedHit, info, _actor.GetObject().transform, _actor.GetPosition());
                index++;
            }
            yield return null;
            timer += Time.deltaTime;
        }

        ReturnObject();
    }

    private IEnumerator Execute(float life)
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

    public void SetEffect(string name, IActor actor = null)
    {
        _actor = actor;
        if (null != actor)
        {
            _action = _dataManager.GetActionInfo(name);
        }
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
