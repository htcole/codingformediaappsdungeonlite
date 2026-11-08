using UnityEngine;
using UnityEngine.AI;

public class EnemyRoam : MonoBehaviour
{
    public float roamRadius = 10f;
    public float waitTime = 2f;

    private NavMeshAgent agent;
    private float waitTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        PickNewDestination();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                PickNewDestination();
                waitTimer = 0f;
            }
        }
    }

    void PickNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            randomDirection,
            out hit,
            roamRadius,
            NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}