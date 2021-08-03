using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BaseEnemy : MonoBehaviour
{
    public Vector3 Position { get { return transform.position; } }
    public NavMeshAgent agent;
    public Animator animator;
    public EnemyStatus status;
    private IActionState currActionState;
    public Transform baseCamp;
    public Player player;

    public void MakeSampleStatus()
    {
        status = new EnemyStatus();
        status.hp = 100;
        status.chaseSpeed = 4;
        status.patrolSpeed = 2.5f;
        status.damage = 5;
        status.attackRange = 1.4f;
        status.attackTerm = 1.0f;
        status.detectionDistance = 6;
        status.chaseDistance = 8;
        status.patrolCycle = 3;
    }

    public void SetFoward(Vector2 dir)
    {
        transform.forward = new Vector3(dir.x, 0, dir.y);
    }
    
    public void LookPlayer()
    {
        var Pos = new Vector3(Position.x, 0, Position.z);
        var PlayerPos = new Vector3(player.Position.x, 0, player.Position.z);
        var dir = (PlayerPos - Pos).normalized;
        transform.forward = dir;
    }
    private void Awake()
    {
        MakeSampleStatus();
        currActionState = new EnemyIdleState(this);
    }
    private void Update()
    {
        currActionState = currActionState.Update();
    }

    public void PlayAnimation(string anim)
    {
        animator.CrossFade(anim, 0.2f, 0, 0f, 0.2f);
    }
}

public struct EnemyStatus
{
    public float hp;
    public float chaseSpeed;
    public float patrolSpeed;
    public float damage;
    public float attackRange;
    public float attackTerm;

    public float detectionDistance;
    public float chaseDistance;
    public float patrolCycle;
}
