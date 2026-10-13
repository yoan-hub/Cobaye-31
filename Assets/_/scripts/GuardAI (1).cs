using UnityEngine;
using UnityEngine.AI;

// Needs: NavMeshAgent, Rigidbody (kinematic), a trigger collider (the guard's "near" zone), 1+ waypoints
public class GuardAI : MonoBehaviour
{
    public Transform[] waypoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;

    public bool isInHiding;   // CES sets this: true when player is in a hiding zone
    public bool chasing;

    NavMeshAgent agent;
    Transform player;
    int index;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        agent.SetDestination(waypoints[index].position);
    }

    void Update()
    {
        if (chasing) Chase(); else Patrol();
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            index = (index + 1) % waypoints.Length;
            agent.SetDestination(waypoints[index].position);
        }
    }

    void Chase()
    {
        // Player hid -> back to patrol
        if (isInHiding)
        {
            chasing = false;
            agent.speed = patrolSpeed;
            agent.SetDestination(waypoints[index].position);
            return;
        }

        agent.SetDestination(player.position);
    }

    // Player is near the guard -> start chasing (if not hidden)
    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.transform;

        if (!chasing && !isInHiding)
        {
            chasing = true;
            agent.speed = chaseSpeed;
        }
    }
}
