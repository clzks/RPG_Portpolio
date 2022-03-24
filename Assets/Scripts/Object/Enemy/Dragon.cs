using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : BaseEnemy
{
    private bool _isFrenzy = false;
    protected float _frenzyTimer;
    protected float _normalAttackTimer = 1f;
    protected float _dashTimer = 7f;
    protected float _fireTimer = 30f;
    protected float _burstTimer = 10f;
    private List<Vector3> _fireHitUnitList = new List<Vector3>();
    //[SerializeField] private Transform _root;
    [SerializeField] private Transform _dragonMouth;
    public override void Init()
    {
        base.Init();

        float interval = 1f;

        int x;
        int z;

        for (int i = 0; i < 9; ++i)
        {
            x = i % 3;
            z = 1 - i / 3;
            Vector3 v = new Vector3(-interval + x, 0, interval + z);
            _fireHitUnitList.Add(v);
        }

    }

    public override void LookPlayer(bool isImmediate = true, float speed = 0.3f)
    {
        base.LookPlayer(isImmediate, speed);
    }

    public override void PlayAnimation(string anim, bool isCrossFade = true)
    {
        base.PlayAnimation(anim);
    }

    public override void ReturnObject()
    {
        base.ReturnObject();
    }

    public override void SetActiveNavMeshAgent(bool enabled)
    {
        base.SetActiveNavMeshAgent(enabled);
    }

    public override void SetEnemy(EnemyInfo info, IActionState actionState)
    {
        base.SetEnemy(info, actionState);
    }

    public override void SummonHitUnit(int index)
    {
        base.SummonHitUnit(index);
    }

    public override void TakeActor(IActor actor, HitUnitStatus hitUnit)
    {
        base.TakeActor(actor, hitUnit);
    }

    public override void TakeDamage(HitUnitStatus hitUnit, ref bool isDead)
    {
        base.TakeDamage(hitUnit, ref isDead);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Update()
    {
        base.Update();
    }
    public void ExecuteDragonEvent()
    {
        StartCoroutine(DragonEvent());
    }

    public bool IsFrenzyHp()
    {
        if(_validStatus.CurrHp / _validStatus.MaxHp <= 0.3f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsFrenzy()
    {
        return _isFrenzy;
    }

    public void SetFrenzy(bool enabled)
    {
        _isFrenzy = enabled;
    }

    public void ExecuteDragonFire(Vector3 position, int level)
    {
        StartCoroutine(DragonFireCoroutine(position, level));
    }

    private IEnumerator DragonFireCoroutine(Vector3 position, int level)
    {
        switch (level)
        {
            case 1:
                break;

            case 2:

                break;

            case 3:

                break;
        }
        var life = _dataManager.GetEffectInfo("DragonMeteor").Life;
        var effect = _objectPool.MakeObject(ObjectType.Effect, "DragonMeteor").GetComponent<BaseEffect>();
        effect.SetEffect("DragonMeteor", this, true);
        effect.SetPosition(Position);
        effect.ExecuteEffect(life);

        yield return null;
    }

    private IEnumerator MakeCustomHitUnit(float startTime, Vector3 position)
    {
        yield return new WaitForSeconds(startTime);

        var hitUnit = MakeHitUnit();

        hitUnit.SetPosition(position);
    }

    public void ExecuteBurstAttack()
    {
        var life = _dataManager.GetEffectInfo("DragonBurst").Life;
        var effect = _objectPool.MakeObject(ObjectType.Effect, "DragonBurst").GetComponent<BaseEffect>();
        effect.SetEffect("DragonBurst", this, true);
        //transform.rotation.ToAngleAxis(out float angle, out Vector3 axis);
        effect.SetPosition(Position + new Vector3(0,1,0));
        //effect.SetRotateAround(transform.position, axis, angle);
        effect.ExecuteEffect(life);

        //StartCoroutine(MakeCustomHitUnit(1f, Position));
    }

    public void ExecuteFlameAttack()
    {
        var info = _dataManager.GetEffectInfo("DragonFlame");
        var life = info.Life;
        var effect = _objectPool.MakeObject(ObjectType.Effect, "DragonFlame").GetComponent<BaseEffect>();
        effect.SetEffect("DragonFlame", this, true);
        if (effect.transform.parent != _dragonMouth.transform)
        {
            effect.SetPosition(_dragonMouth.position);
            effect.SetParent(_dragonMouth);
            effect.transform.localEulerAngles = new Vector3(22f, -90f, 4f);
        }
        
        effect.ExecuteEffect(life);
        StartCoroutine(UpdateDragonFlameHitUnit(life, effect));
    }

    private IEnumerator UpdateDragonFlameHitUnit(float life, BaseEffect effect)
    {
        float timer = 0f;

        while (timer <= life)
        {
            for (int i = 0; i < 10; ++i)
            {
                //if (timer < 0.2f && i > 5) 
                //{
                //    continue;
                //}
                //
                //if (timer < 0.4f && i > 7) 
                //{
                //    continue;
                //}

                var hitUnit = MakeHitUnit();

                if (null != hitUnit && null != effect)
                {
                    hitUnit.SetPosition(effect.GetPosition() + effect.transform.forward * i * 1.5f);
                }
            }

            yield return new WaitForSeconds(0.15f);
            timer += 0.15f;
        }
    }


    private IEnumerator DragonEvent()
    {
        yield return null;
        //InGameCamera camera = Camera.main.GetComponent<InGameCamera>();
        //
        //camera.SetActiveUI(false);
        //_player.SetFollowCamera(false);
        //_player.ResetMovePad();
        //yield return camera.TurnOff(0.5f);
        //yield return new WaitForSeconds(1f);
        //// 용 위치로 카메라 이동
        //camera.SetCameraTransform(Position, new Vector3(0, 5.5f, 5.5f), new Vector3(30, 180, 0));
        //
        //yield return camera.TurnOn(0.5f);
        //yield return new WaitForSeconds(1f);
        //PlayAnimation("Scream");
        //yield return new WaitForSeconds(2.5f);
        //// 크아아아앙 
        //
        //yield return camera.TurnOff(0.5f);
        //yield return new WaitForSeconds(1f);
        //
        //// 카메라 원상복구
        //camera.ResetRotation();
        //_player.SetFollowCamera(true);
        //yield return camera.TurnOn(0.5f);
        //yield return new WaitForSeconds(1f);
        //
        //camera.SetActiveUI(true);
        currActionState = currActionState.ChangeState(new DragonChaseState(this));
    }

    public void UpdateAttackTimer()
    {
        _normalAttackTimer -= Time.deltaTime;
        _dashTimer -= Time.deltaTime;
        _fireTimer -= Time.deltaTime;

        if (true == _isFrenzy)
        {
            _normalAttackTimer -= Time.deltaTime;
            _dashTimer -= Time.deltaTime;
        }
    }

    public void UpdateBurstTimer()
    {
        _burstTimer -= Time.deltaTime;
    }

    public float GetNormalTimer()
    {
        return -1;
        //return _normalAttackTimer;
    }

    public float GetDashTimer()
    {
        return _dashTimer;
    }

    public float GetFireTimer()
    {
        return _fireTimer;
    }

    public float GetBurstTimer()
    {
        return _burstTimer;
    }

    public void ResetNormalTimer()
    {
        _normalAttackTimer = 1f;
    }

    public void ResetDashTimer()
    {
        _dashTimer = 7f;
    }

    public void ResetFireTimer()
    {
        _fireTimer = 25f;
    }

    public void ResetBurstTimer()
    {
        _burstTimer = 10f;
    }

    public override HitUnit MakeHitUnit()
    {
        return base.MakeHitUnit();
    }
}
