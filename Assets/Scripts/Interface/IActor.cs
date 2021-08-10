using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActor : IPoolObject
{
    public void TakeActor(IActor actor, HitUnitStatus status);
    public void TakeDamage(HitUnitStatus status);
    public void ResetActorList();
    public int GetId();
    public float GetDamage();
    public void MoveCharacter(float animTime, float distance, Vector3 dir);
}
