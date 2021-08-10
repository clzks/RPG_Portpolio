using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo
{
    public Vector3 actorPos;
    public float distance;
    public float stiffNessTime;
    public float stiffNessDelay;
    public DamageInfo(Vector3 actorPosition, float knockBackdistance, float time)
    {
        actorPos = actorPosition;
        distance = knockBackdistance;
        stiffNessTime = time;
        stiffNessDelay = 0.3f;
    }
}
