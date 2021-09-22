using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActor : IPoolObject
{
    void TakeActor(IActor actor, HitUnitStatus status);
    void TakeDamage(HitUnitStatus status, ref bool isDead);
    void ResetActorList();
    int GetId();
    float GetDamage();
    float GetHpPercent();
    void MoveCharacter(float animTime, float distance, Vector3 dir);
    void RemoveBuff(IBuff buff);
    Status GetValidStatus();
}
