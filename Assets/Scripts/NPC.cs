using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public float wanderRadius = 10f;

    private NavMeshAgent navMeshAgent;
    private Vector3 wanderTarget;
    public Animator animator;

    void Start()
    {
        // Get the NavMeshAgent component
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Set the initial wander target
        SetRandomWanderTarget();
    }

    void Update()
    {
        // If not chasing, wander around
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            // Set a new random wander target when reached the current target
            SetRandomWanderTarget();
        }
    }

    // Set a random wander target within the specified wander radius
    void SetRandomWanderTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, -1);
        wanderTarget = navHit.position;
        navMeshAgent.SetDestination(wanderTarget);
        animator.SetBool("isWalking", true);
    }
}
