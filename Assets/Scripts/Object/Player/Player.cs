using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif
public class Player : MonoBehaviour, IActor
{
    [SerializeField]private InGameCamera _camera;
    [SerializeField]private Animator _animController;
    private SphereCollider _collider;
    public VirtualGamePad movePad;
    public ActionButton attackButton;
    public float speed = 3f;
    public IActionState currActionState;
    private Dictionary<string, ActionInfo> _actionInfoList;
    public GameObject hitUnitPrefab;
    private List<IActor> _actorList;
    private Status _status;
    private DamageInfo _damageInfo;
    private IEnumerator _moveCoroutine = null;

    public Vector3 Position { get { return transform.position; } }
    
    public enum ActionType
    { 
        Idle,
        Run,
        Attack,
        Damage,
        Skill,
        Die,
        Count
    }

    public enum AnimationType
    { 
        Attack01,
        Attack02,
        Attack03,
        Slash01,
        Slash02,
        Slash03,
        Idle,
        Damage,
        Run,
        Casting,
        Die
    }

    private void Awake()
    {
        _camera.SetCameraDistance(transform.position);
        currActionState = new PlayerIdleState(this);
        _actionInfoList = DataManager.Get().GetActionInfoList();
        _collider = GetComponent<SphereCollider>();
        _actorList = new List<IActor>();
        _status = Status.MakeSampleStatus();
        _damageInfo = null;
    }

    private void LateUpdate()
    {
        _camera.FollowPlayer(transform.position);
    }

    private void Update()
    {
        currActionState = currActionState.Update();
    }

    public void PlayAnimation(string anim)
    {
        _animController.CrossFade(anim, 0.2f, 0, 0f, 0.2f);
    }

    public void SetForward(Vector2 dir)
    {
        transform.forward = new Vector3(dir.x, 0, dir.y);
    }

    public Vector3 GetForward()
    {
        return transform.forward;
    }

    public void MovePlayerByPad()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public bool OnClickAttackButton()
    {
        return true;
    }

    public void SummonHitUnit(int index)
    {
#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name == "AnimationEditorScene")
        {
            return;
        }
#endif
        var state = currActionState as PlayerActionState;
        var actionInfo = state.GetActionInfo();

        if (null == actionInfo)
        {
            return;
        }

        if (actionInfo.HitUnitList.Count <= index)
        {
            return;
        }
        HitUnit hitUnit = Instantiate(hitUnitPrefab).GetComponent<HitUnit>();
        HitUnitInfo info = actionInfo.HitUnitList[index];
        hitUnit.SetHitUnit(this, info, transform);
    }

    public void TakeActor(IActor actor, HitUnitStatus hitUnit)
    {
        if (false == _actorList.Contains(actor))
        {
            actor.TakeDamage(hitUnit);
            _actorList.Add(actor);
        }
    }

    public Animator GetAnimator()
    {
        return _animController;
    }

    public ActionInfo GetActionInfo(string key)
    {
        if(_actionInfoList.ContainsKey(key))
        {
            return _actionInfoList[key];
        }
        else
        {
            return null;
        }
    }

    public float GetColRadius()
    {
        return _collider.radius;
    }

    public int GetId()
    {
        return -1;
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public ObjectType GetObjectType()
    {
        return ObjectType.Player;
    }

    public void Init()
    {
        
    }

    public void ReturnObject()
    {
      
    }

    public void TakeDamage(HitUnitStatus hitUnit)
    {
        Debug.Log("플레이어에게 데미지 " + hitUnit.Damage + "만큼입힘");
        _status.Hp -= hitUnit.Damage;
        
        // TODO 데미지 이펙트 추가할 곳

        // 넉백 및 경직이 없다는 뜻
        if(0f >= hitUnit.Strength)
        {
            return;
        }

        if(null == _damageInfo && false == _status.IsInvincible)
        {
            _damageInfo = new DamageInfo(hitUnit.Position, hitUnit.Strength, hitUnit.Strength * 0.2f);
        }
    }

    public void ResetDamageInfo()
    {
        _damageInfo = null;
    }

    public DamageInfo GetDamageInfo()
    {
        return _damageInfo;
    }

    public float GetDamage()
    {
        return 3f;
    }

    public void ResetActorList()
    {
        _actorList.Clear();
    }

    public void MoveCharacter(float time, float distance, Vector3 dir)
    {
        if (null != _moveCoroutine)
        {
            StopCoroutine(_moveCoroutine);
        }
        _moveCoroutine = MoveCoroutine(time, distance, dir);
        StartCoroutine(_moveCoroutine);
    }

    public void MoveCharacter(float delayTime, float time, float distance, Vector3 dir)
    {
        if (null != _moveCoroutine)
        {
            StopCoroutine(_moveCoroutine);
        }
        _moveCoroutine = MoveCoroutine(delayTime, time, distance, dir);
        StartCoroutine(_moveCoroutine);
    }

    private IEnumerator MoveCoroutine(float time, float distance, Vector3 dir)
    {
        float timer = 0f;
        while (timer <= time)
        { 
            timer += Time.deltaTime;
            transform.position += (Time.deltaTime * distance / time)  * dir;
            yield return new WaitForEndOfFrame();
        }

        _moveCoroutine = null;
    }

    private IEnumerator MoveCoroutine(float delayTime, float time, float distance, Vector3 dir)
    {
        yield return new WaitForSeconds(delayTime);
        float timer = 0f;
        while (timer <= time)
        {
            timer += Time.deltaTime;
            transform.position += (Time.deltaTime * distance / time) * dir;
            yield return new WaitForEndOfFrame();
        }

        _moveCoroutine = null;
    }
}