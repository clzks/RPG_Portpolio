using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo
{
    public Vector3 actorPos;
    public float distance;
    public float knockBackTime;
    public float knockBackDelay;
    public DamageInfo(Vector3 actorPosition, float knockBackdistance)
    {
        actorPos = actorPosition;
        distance = knockBackdistance;
        knockBackTime = 0.2f;
        knockBackDelay = 0.3f;
    }
}
