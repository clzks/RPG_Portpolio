using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif
public class Player : MonoBehaviour, IActor
{
    private GameManager _gameManager;
    private ObjectPoolManager _objectPool;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private InGameCamera _camera;
    [SerializeField] private Animator _animController;
    private SphereCollider _collider;
    [Header("UI")]
    [SerializeField] private MovePad _movePad;
    [SerializeField] private ActionPad _actionPad;
    //[SerializeField] private ActionButton _attackButton;
    [SerializeField] private TargetInfoPanel _targetPanel;
    [SerializeField] private PlayerFieldStatusUI _fieldStatusUI;
    [SerializeField] private MiniMap _miniMap;
    [Header("Status")]
    public float speed = 30f;
    public IActionState currActionState;
    private Dictionary<string, ActionInfo> _actionInfoList;
    //public GameObject hitUnitPrefab;
    private List<IActor> _actorList;
    private List<IBuff> _buffList;
    private Status _originStatus;
    private Status _validStatus;
    private DamageInfo _damageInfo;
    private IEnumerator _moveCoroutine = null;
    private bool _followCamera = true;
    private bool _isInBattle = false;
    private int _currNormalAttackCount = 0;
    public Vector3 Position { get { return transform.position; } }
    private WaitForSeconds _buffYield;
    private float _tick;
    private void Awake()
    {
        _gameManager = GameManager.Get();
        _objectPool = ObjectPoolManager.Get();
        //_camera.SetCameraDistance(transform.position);
        currActionState = new PlayerIdleState(this);
        _actionInfoList = DataManager.Get().GetActionInfoList();
        _collider = GetComponent<SphereCollider>();
        _actorList = new List<IActor>();
        _buffList = new List<IBuff>();
        _originStatus = Status.MakeSampleStatus();
        _validStatus = _originStatus;
        _damageInfo = null;
        _tick = _gameManager.tick;
        _buffYield = new WaitForSeconds(_tick);
    }

    private void LateUpdate()
    {
        var array = GetEnemyPosArray();

        if (true == _followCamera)
        {
            _camera.FollowPlayer(transform.position);
            _miniMap.MiniMapUpdate(array, new Vector4(Position.x, Position.z, 0, 0) / 100f);
        }
    }

    private void Update()
    {
        currActionState = currActionState.Update();

        _fieldStatusUI.SetStatusPanel(GetHpPercent(), 1);

        if(Input.GetKeyDown(KeyCode.C))
        {
            _followCamera = !_followCamera;
        }
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
        HitUnit hitUnit = _objectPool.MakeObject(ObjectType.HitUnit, "NormalHitUnit").GetComponent<HitUnit>();
        HitUnitInfo info = actionInfo.HitUnitList[index];
        hitUnit.SetHitUnit(this, info, transform);
    }

    public void TakeActor(IActor actor, HitUnitStatus hitUnit)
    {
        bool isKill = false;
        if (false == _actorList.Contains(actor))
        {
            actor.TakeDamage(hitUnit, ref isKill);
            _actorList.Add(actor);
        }

        _targetPanel.SetTargetInfo(actor, isKill);
    }
    #region GET SET
    public Vector3 GetPosition()
    {
        return Position;
    }
    public Animator GetAnimator()
    {
        return _animController;
    }

    public MovePad GetVirtualGamePad()
    {
        return _movePad;
    }

    public ActionPad GetActionPad()
    {
        return _actionPad;
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
    public string GetName()
    {
        return "Player";
    }

    public DamageInfo GetDamageInfo()
    {
        return _damageInfo;
    }

    public float GetDamage()
    {
        return 3f;
    }

    public float GetHpPercent()
    {
        return _originStatus.CurrHp / _originStatus.MaxHp;
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return _agent;
    }

    public bool GetInBattle()
    {
        return _isInBattle;
    }

    public int GetCurrNormalAttackCount()
    {
        return _currNormalAttackCount;
    }

    public Status GetValidStatus()
    {
        return _validStatus;
    }
    #endregion
    public void Init()
    {
        
    }

    public void ReturnObject()
    {
      
    }

    public void TakeDamage(HitUnitStatus hitUnit, ref bool isDead)
    {
        //Debug.Log("플레이어에게 데미지 " + hitUnit.Damage + "만큼입힘");
        _originStatus.CurrHp -= hitUnit.Damage;

        // TODO 데미지 이펙트 추가할 곳
        var damageText = _objectPool.MakeObject(ObjectType.DamageText, "DamageText").GetComponent<DamageText>();
        damageText.SetText(DamageTextType.Player, (int)hitUnit.Damage, Position);
        damageText.ExecuteFloat();

        // 넉백 및 경직이 없다는 뜻
        if (0f >= hitUnit.Strength)
        {
            return;
        }

        if(null == _damageInfo && false == _originStatus.IsInvincible)
        {
            _damageInfo = new DamageInfo(hitUnit.ActorPosition, hitUnit.Strength, hitUnit.Strength * 0.2f);
        }

        if(_originStatus.CurrHp <= 0)
        {
            isDead = true;
        }
        else
        {
            isDead = false;
        }
    }

    public void ResetDamageInfo()
    {
        _damageInfo = null;
    }

    public void ResetActorList()
    {
        _actorList.Clear();
    }

    public void SetActiveNavMeshAgent(bool enabled)
    {
        _agent.enabled = enabled;
    }

    public void SetInBattle(bool enabled)
    {
        _isInBattle = enabled;
    }


    public void AddNormalAttackCount(int value)
    {
        _currNormalAttackCount += value;
    }

    public void ResetNormalAttackCount()
    {
        _currNormalAttackCount = 0;
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

    private Vector4[] GetEnemyPosArray()
    {
        Vector4[] array = new Vector4[50];
        var list = _objectPool.GetEnemies();
        list = list.FindAll(x => (x.GetPosition() - Position).magnitude <= 30f);

        for(int i = 0; i < 50; ++i)
        {
            if(list.Count - 1 < i)
            {
                array[i] = new Vector4(0, 0, 0, -100) / 100f;
                break;
            }
            else
            {
                var enemyPos = list[i].GetPosition();
                array[i] = new Vector4(enemyPos.x, enemyPos.y, enemyPos.z, 100) / 100f; 
            }
        } 

        return array;
    }

    private IEnumerator StatusUpdate()
    {
        while (true)
        {
            _validStatus = _originStatus;

            if (null != _buffList)
            {
                foreach (var buff in _buffList)
                {
                    buff.Update(_tick, this);
                }
            }
            yield return _buffYield;
        }
    }

    public void RemoveBuff(IBuff buff)
    {
        _buffList.Remove(buff);
    }

   
}