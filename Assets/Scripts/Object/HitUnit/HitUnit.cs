using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class HitUnit : MonoBehaviour, IPoolObject
{
    private ObjectPoolManager _objectPool;
    private CancellationTokenSource _disableCancellation = new CancellationTokenSource();
    private ObjectType _type = ObjectType.HitUnit;
    public SphereCollider sphereCollider;
    public float colRadius;
    public float lifeTime = 0.5f;

    private void OnEnable()
    {
        if (_disableCancellation != null)
        {
            _disableCancellation.Dispose();
        }
        _disableCancellation = new CancellationTokenSource();

        if (null ==  _objectPool)
        {
            _objectPool = ObjectPoolManager.Get();
        }
    }

    private void Start()
    {
        Execute().Forget();
    }

    public void SetHitUnit(HitUnitInfo info, Transform actorTransform)
    {
        lifeTime = info.Life;
        colRadius = info.ColliderRadius;

        actorTransform.rotation.ToAngleAxis(out float angle, out Vector3 axis);
        transform.position = actorTransform.position + new Vector3(info.SidePos, 0f, info.FrontPos);
        transform.RotateAround(actorTransform.position, axis, angle);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + new Vector3(0, 0.0f, 0f), colRadius);
    }

    private async UniTaskVoid Execute()
    {
        float timer = 0f;

        while(timer <= lifeTime)
        {
            timer += Time.deltaTime;
            await UniTask.Yield(_disableCancellation.Token);
        }

        Destroy(gameObject);
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public ObjectType GetObjectType()
    {
        return _type;
    }

    public void Init()
    {
        
    }

    public void ReturnObject()
    {
        _objectPool.ReturnObject(this);
    }
}
