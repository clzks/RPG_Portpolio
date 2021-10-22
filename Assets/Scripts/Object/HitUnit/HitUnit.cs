using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class HitUnit : MonoBehaviour, IPoolObject
{
    // UniTask 사용하므로 토큰 만들어줘야함
    private IActor _actor;
    private ObjectPoolManager _objectPool;
    private CancellationTokenSource _disableCancellation = new CancellationTokenSource();
    private ObjectType _type = ObjectType.HitUnit;
    public Vector3 Position { get { return transform.position; } }
    public SphereCollider sphereCollider;
    public float lifeTime = 0.5f;
    public HitUnitStatus status;
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

        Execute().Forget();
    }

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        var actor = other.GetComponent<IActor>();

        _actor.TakeActor(actor, status);
    }

    public void SetHitUnit(IActor actor, HitUnitInfo info, Transform actorTransform)
    {
        _actor = actor;
        gameObject.layer = info.Layer;
        lifeTime = info.Life;
        sphereCollider.radius = info.ColliderRadius;
        status.ActorPosition = actorTransform.position;
        status.Damage = info.DamageFactor * _actor.GetAttackValue();
        status.Strength = info.StrengthFactor;
        actorTransform.rotation.ToAngleAxis(out float angle, out Vector3 axis);
        transform.position = actorTransform.position + new Vector3(info.SidePos, 0f, info.FrontPos);
        transform.RotateAround(actorTransform.position, axis, angle);
    }
#if UNITY_EDITOR
    public void SetSampleHitUnit(HitUnitInfo info, Transform actorTransform)
    {
        lifeTime = info.Life;
        sphereCollider.radius = info.ColliderRadius;
        //status.Damage = info.DamageFactor * _actor.GetDamage();
        //status.Strength = info.StrengthFactor;
        actorTransform.rotation.ToAngleAxis(out float angle, out Vector3 axis);
        transform.position = actorTransform.position + new Vector3(info.SidePos, 0f, info.FrontPos);
        transform.RotateAround(actorTransform.position, axis, angle);
    }
#endif

    private async UniTaskVoid Execute()
    {
        float timer = 0f;

        while(timer <= lifeTime)
        {
            timer += Time.deltaTime;
            await UniTask.Yield(_disableCancellation.Token);
        }

        ReturnObject();
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public ObjectType GetObjectType()
    {
        return _type;
    }

    public void ReturnObject()
    {
        _objectPool.ReturnObject(this);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + new Vector3(0, 0.0f, 0f), sphereCollider.radius);
    }

    public string GetName()
    {
        return "NormalHitUnit";
    }

    public Vector3 GetPosition()
    {
        return Position;
    }
}

public struct HitUnitStatus
{
    public Vector3 ActorPosition;
    public float Damage;
    public float Strength;
}