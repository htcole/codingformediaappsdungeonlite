using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ProceduralEnemyAI : MonoBehaviour
{
    public enum AIState { Roam, Chase }
    public AIState currentState = AIState.Roam;

    [Header("Roaming Settings")]
    public float roamRadius = 10f;
    public float waitTimeAtTarget = 2f;
    private float waitTimer;

    [Header("Detection Settings")]
    public float detectionRange = 8f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    private Transform player;

    [Header("Scaling (per generation cleared)")]
    public float speedPerRun = 0.1f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // The first dungeon is generation 1, so nothing has been cleared yet
        int generationsCleared = Mathf.Max(0, DungeonGenerator.runCount - 1);
        float speedMultiplier = 1f + speedPerRun * generationsCleared;

        // Acceleration and turning scale too, or faster enemies would lag behind their own speed
        agent.speed *= speedMultiplier;
        agent.acceleration *= speedMultiplier;
        agent.angularSpeed *= speedMultiplier;

        SetNewRandomRoamTarget();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if player is detected
        if (distanceToPlayer <= detectionRange && CanSeePlayer())
        {
            currentState = AIState.Chase;
        }
        else if (currentState == AIState.Chase && distanceToPlayer > detectionRange * 1.5f)
        {
            // Lose player and go back to roaming
            currentState = AIState.Roam;
            SetNewRandomRoamTarget();
        }

        // State Machine Logic
        switch (currentState)
        {
            case AIState.Roam:
                RoamBehavior();
                break;
            case AIState.Chase:
                ChaseBehavior();
                break;
        }
    }

    void RoamBehavior()
    {
        // Check if agent reached its destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtTarget)
            {
                SetNewRandomRoamTarget();
                waitTimer = 0f;
            }
        }
    }

    void ChaseBehavior()
    {
        agent.SetDestination(player.position);
    }

    void SetNewRandomRoamTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position + Vector3.up) - (transform.position + Vector3.up);
        float distanceToPlayer = directionToPlayer.magnitude;

        // Raycast to check if an obstacle blocks the view
        if (!Physics.Raycast(transform.position + Vector3.up, directionToPlayer.normalized, distanceToPlayer, obstacleLayer))
        {
            return true;
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, roamRadius);
    }
}