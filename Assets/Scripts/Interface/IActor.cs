using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActor : IPoolObject
{
    void TakeActor(IActor actor, HitUnitStatus status);
    void TakeDamage(HitUnitStatus status, ref bool isDead);
    void ResetActorList();
    int GetId();
    float GetAttackValue();
    float GetHpPercent();
    void MoveCharacter(float animTime, float distance, Vector3 dir);
    bool AddBuff(IBuff buff);
    void RemoveBuff(IBuff buff);
    Status GetValidStatus();
    Status GetOriginStatus();
    float GetShield();
    void ResetShield();
    bool IsInvincible();
}
