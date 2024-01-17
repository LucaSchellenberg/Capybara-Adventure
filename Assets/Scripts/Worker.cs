using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5f;
    public float chaseRadius = 10f;
    public float wanderRadius = 10f;
    public float chasingSpeed = 1.5f;  // New variable for chasing speed

    private NavMeshAgent navMeshAgent;
    private Vector3 wanderTarget;
    private bool isChasing = false;
    private float originalSpeed;  // Variable to store the original speed
    public Animator animator;

    void Start()
    {
        // Get the NavMeshAgent component
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Store the original speed
        originalSpeed = navMeshAgent.speed;

        // Set the initial wander target
        SetRandomWanderTarget();
    }

    void Update()
    {
        // Check the distance between the worker and the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If the player is within the chase radius, start chasing
        if (distanceToPlayer <= chaseRadius)
        {
            isChasing = true;
            animator.SetBool("isChasing", true);

        }

        if (!isChasing)
        {
            // If not chasing, wander around
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
            {
                // Set a new random wander target when reached the current target
                SetRandomWanderTarget();
                animator.SetBool("isWalking", true);
                animator.SetBool("isChasing", false);
            }

        }
        else
        {
            // If chasing, set the destination to the player's position
            navMeshAgent.SetDestination(player.position);

            // If the player is out of detection range, stop chasing
            if (distanceToPlayer > chaseRadius)
            {
                isChasing = false;
                SetRandomWanderTarget();
            }
        }

        // Adjust the speed when chasing
        navMeshAgent.speed = isChasing ? chasingSpeed : originalSpeed;
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

    void OnCollisionEnter(Collision collision)
    {
        // Check if the worker collided with the player
        if (collision.collider.CompareTag("Worker"))
        {
            // Set the player's position to the specified coordinates
            player.position = new Vector3(-11f, 0.88f, 74f);
        }
    }
}
