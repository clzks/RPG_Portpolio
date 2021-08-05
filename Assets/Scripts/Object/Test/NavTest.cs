using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NavTest : MonoBehaviour
{
    private NavMeshAgent navMesh;
    public Transform target;

    private void Awake()
    {
        navMesh = GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            navMesh.SetDestination(target.position);
        }
    }
}
