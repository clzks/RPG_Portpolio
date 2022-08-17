using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class HitUnit : MonoBehaviour, IPoolObject
{
    // UniTask 사용하므로 토큰 만들어줘야함
    private string _name;
    private IActor _actor;
    private ObjectPoolManager _objectPool;
    //private CancellationTokenSource _disableCancellation = new CancellationTokenSource();
    private ObjectType _type = ObjectType.HitUnit;
    public Vector3 Position { get { return transform.position; } }
    public SphereCollider sphereCollider;
    //public float lifeTime = 0.5f;
    private HitUnitStatus status;
    private HitUnitInfo _info;
    private float _timer = 0f;

    private void Awake()
    {
        DonDestroy();
    }
    private void OnEnable()
    {
        //if (_disableCancellation != null)
        //{
        //    _disableCancellation.Dispose();
        //}
        //_disableCancellation = new CancellationTokenSource();

        if (null ==  _objectPool)
        {
            _objectPool = ObjectPoolManager.Get();
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_info.Life <= _timer)
        {
            ReturnObject();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var actor = other.GetComponent<IActor>();

        // 무적이 아닌 경우 피격
        if (false == actor.IsInvincible())
        {
            _actor.TakeActor(actor, status);
        }
    }

    /// <summary>
    /// HitUnit을 설정하는 함수
    /// </summary>
    /// <param name="actor">시전 객체</param>
    /// <param name="duplicatedHit">중복 타격이 가능한가</param>
    /// <param name="info">HitUnit 정보</param>
    /// <param name="actorTransform">시전 객체 Transform</param>
    /// <param name="rootPosition">HitUnit 소환 위치</param>
    public void SetHitUnit(IActor actor, bool duplicatedHit, HitUnitInfo info, Transform actorTransform, Vector3 rootPosition)
    {
        _actor = actor;
        gameObject.layer = info.Layer;
        _info = info;
        sphereCollider.radius = info.ColliderRadius;
        status.ActorPosition = rootPosition;
        status.Damage = info.DamageFactor * _actor.GetAttackValue();
        status.Strength = info.StrengthFactor;
        status.DuplicatedHit = duplicatedHit;
        actorTransform.rotation.ToAngleAxis(out float angle, out Vector3 axis);
        transform.position = rootPosition + new Vector3(info.SidePos, 0f, info.FrontPos);
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        transform.RotateAround(rootPosition, axis, angle);
    }

    public void SetHitUnit(IActor actor, bool duplicatedHit, HitUnitInfo info)
    {
        _actor = actor;
        gameObject.layer = info.Layer;
        _info = info;
        sphereCollider.radius = info.ColliderRadius;
        status.ActorPosition = actor.GetPosition();
        status.Damage = info.DamageFactor * _actor.GetAttackValue();
        status.Strength = info.StrengthFactor;
        status.DuplicatedHit = duplicatedHit;
        transform.position = status.ActorPosition;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, 0f, pos.z);
    }
#if UNITY_EDITOR
    public void SetSampleHitUnit(HitUnitInfo info, Transform actorTransform, Vector3 rootPosition)
    {
        _info = info;
        sphereCollider.radius = info.ColliderRadius;
        //status.Damage = info.DamageFactor * _actor.GetDamage();
        //status.Strength = info.StrengthFactor;
        actorTransform.rotation.ToAngleAxis(out float angle, out Vector3 axis);
        transform.position = rootPosition + new Vector3(info.SidePos, 0f, info.FrontPos);
        transform.RotateAround(rootPosition, axis, angle);
    }
#endif

    //private async UniTaskVoid Execute()
    //{
    //    float timer = 0f;
    //
    //    while(timer <= _info.Life)
    //    {
    //        timer += Time.deltaTime;
    //        await UniTask.Yield(_disableCancellation.Token);
    //    }
    //
    //    ReturnObject();
    //}


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
        _timer = 0f;
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

    public void DonDestroy()
    {
        DontDestroyOnLoad(gameObject);
    }
}

public struct HitUnitStatus
{
    public Vector3 ActorPosition;
    public float Damage;
    public float Strength;
    public bool DuplicatedHit;
}