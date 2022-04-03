using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : BaseEnemy
{
    protected float _dashTimer = 5f;
    protected float _burstTimer = 4f;
    protected float _meteorTimer = 15f;
    protected float _flameTimer = 5f;
    [SerializeField] private Transform _dragonMouth;
  
    private List<Vector3> _metoerSpotList = new List<Vector3>();
    private int _difficulty = 1;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        float interval = 3f;

        int x;
        int z;

        for (int i = 0; i < 9; ++i)
        {
            x = i % 3;
            z = 1 - i / 3;
            Vector3 v = new Vector3((-1 + x ) * interval, 0,  z * interval);
            _metoerSpotList.Add(v);
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
    
    public int GetDragonProDiff()
    {
        float per = GetHpPercent();

        if(per >= 0.7f)
        {
            return 1;
        }
        else if(per >= 0.35f)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }

    public int GetDragonCurrDiff()
    {
        return _difficulty;
    }

    public void ChangeDifficulty()
    {
        _difficulty = GetDragonProDiff();
    }

    public void ExecuteMeteorAttack(int level)
    {
        StartCoroutine(UpdateDragonMeteor(level));
    }

    
    private IEnumerator UpdateDragonMeteor(int level)
    {
        // 드래곤의 공중공격시간은 대략 2초정도로 계산하고 여유있게 착륙시킨다. 
        var life = _dataManager.GetEffectInfo("DragonMeteor").Life;

        int r = Random.Range(0, 100);

        Vector3 playerPos = GetPlayer().Position;

        switch (level)
        {
            case 1:
                // 대충 1초대
                if (r % 2 == 0)
                {
                    for (int i = 3; i < 6; ++i)
                    {
                        MakeDragonMeteorByIndex(playerPos, i, life);
                        yield return new WaitForSeconds(0.3f);
                    }
                }
                else
                {
                    for (int i = 0; i < 2; ++i)
                    {
                        MakeDragonMeteorByIndex(playerPos, 4, life);
                        yield return new WaitForSeconds(0.4f);
                    }
                }

                break;

            case 2:
                // 대충 2초?
                if (r % 2 == 0)
                {
                    for (int i = 3; i < 6; ++i)
                    {
                        MakeDragonMeteorByIndex(playerPos, i, life);
                        yield return new WaitForSeconds(0.2f);
                    }

                    yield return new WaitForSeconds(0.4f);
                    playerPos = GetPlayer().Position;

                    for (int i = 3; i < 6; ++i)
                    {
                        MakeDragonMeteorByIndex(playerPos, i, life);
                        yield return new WaitForSeconds(0.2f);
                    }
                }
                else
                {
                    for(int i = 0; i < 4; ++i)
                    {
                        MakeDragonMeteorByIndex(playerPos, i * 2 + 1, life);
                        yield return new WaitForSeconds(0.15f);
                    }

                    MakeDragonMeteorByIndex(playerPos, 4, life);
                }
                break;

            case 3:
                // 대충 2초
                MakeDragonMeteorByIndex(playerPos, 0, life);
                yield return new WaitForSeconds(0.2f);
                MakeDragonMeteorByIndex(playerPos, 1, life);
                MakeDragonMeteorByIndex(playerPos, 3, life);
                yield return new WaitForSeconds(0.2f);
                MakeDragonMeteorByIndex(playerPos, 2, life);
                MakeDragonMeteorByIndex(playerPos, 4, life);
                MakeDragonMeteorByIndex(playerPos, 6, life);
                yield return new WaitForSeconds(0.2f);
                MakeDragonMeteorByIndex(playerPos, 5, life);
                MakeDragonMeteorByIndex(playerPos, 7, life);
                yield return new WaitForSeconds(0.2f);
                MakeDragonMeteorByIndex(playerPos, 8, life);
                break;
        }
    }

    private void MakeDragonMeteorByIndex(Vector3 playerPos, int index, float life)
    {
        var effect = _objectPool.MakeObject(ObjectType.Effect, "DragonMeteor").GetComponent<BaseEffect>();
        effect.SetEffect("DragonMeteor", this, true);
        effect.SetPosition(playerPos + _metoerSpotList[index]);
        effect.ExecuteEffect(life);
        StartCoroutine(MakeCustomHitUnit(0.7f, effect.GetPosition()));
    }

    private IEnumerator MakeCustomHitUnit(float startTime, Vector3 position)
    {
        yield return new WaitForSeconds(startTime);

        var hitUnit = MakeHitUnit(_dataManager.GetEnemyActionInfo("Dragon", "DragonMeteor"));
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
                var hitUnit = MakeHitUnit(null);

                if (null != hitUnit && null != effect)
                {
                    hitUnit.SetPosition(effect.GetPosition() + effect.transform.forward * i * 1.5f);
                }
            }

            yield return new WaitForSeconds(0.15f);
            timer += 0.15f;
        }
    }

    public void ExecuteDustEffect()
    {
        var info = _dataManager.GetEffectInfo("DragonDust");
        var life = info.Life;
        var effect = _objectPool.MakeObject(ObjectType.Effect, "DragonDust").GetComponent<BaseEffect>();
        effect.SetPosition(Position + new Vector3(0, 3f, 0));
        effect.ExecuteEffect(life);
    }

    public void ExecuteFrenzyEffect()
    {
        var info = _dataManager.GetEffectInfo("DragonFrenzy");
        var effect = _objectPool.MakeObject(ObjectType.Effect, "DragonFrenzy").GetComponent<BaseEffect>();
        effect.SetPosition(RootPosition);
        effect.SetParent(GetRootTransform());
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
        _dashTimer -= Time.deltaTime;
        _meteorTimer -= Time.deltaTime;

        if (2 == _difficulty)
        {
            _flameTimer -= Time.deltaTime;
            _meteorTimer -= Time.deltaTime;
        }
        else if(3 == _difficulty)
        {
            _burstTimer -= Time.deltaTime;
            _flameTimer -= Time.deltaTime;
            _dashTimer -= Time.deltaTime;
            _meteorTimer -= Time.deltaTime;
        }
    }

    public float GetDashTimer()
    {
        return _dashTimer;
    }

    public float GetMeteorTimer()
    {
        return _meteorTimer;
    }

    public float GetFlameTimer()
    {
        return _flameTimer;
    }

    public float GetBurstTimer()
    {
        return _burstTimer;
    }

    public void ResetDashTimer()
    {
        _dashTimer = 5f;
    }

    public void ResetBurstTimer()
    {
        _burstTimer = 4f;
    }

    public void ResetMeteorTimer()
    {
        _meteorTimer = 20f;
    }

    public void ResetFlameTimer()
    {
        _flameTimer = 5f;
    }

    public void AddMeteorTime(float time)
    {
        _meteorTimer += time;
    }

    public void AddFlameTime(float time)
    {
        _flameTimer += time;
    }

    public void AddBurstTime(float time)
    {
        _burstTimer += time;
    }

    public override HitUnit MakeHitUnit(EnemyAction action)
    {
        return base.MakeHitUnit(action);
    }
}
