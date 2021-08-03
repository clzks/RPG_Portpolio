using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
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

    public void SetFoward(Vector2 dir)
    {
        transform.forward = new Vector3(dir.x, 0, dir.y);
    }

    public void MovePlayer()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public bool OnClickAttackButton()
    {
        return true;
    }

    public void SummonHitUnit(int index)
    {
        var state = currActionState as PlayerActionState;
        var actionInfo = state.GetActionInfo();

        if (actionInfo.HitUnitList.Count <= index)
        {
            return;
        }
        HitUnit hitUnit = Instantiate(hitUnitPrefab).GetComponent<HitUnit>();
        HitUnitInfo info = actionInfo.HitUnitList[index];
        hitUnit.SetHitUnit(info, transform);
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
}
