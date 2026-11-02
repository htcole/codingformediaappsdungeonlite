/*Tutorials used:

Tutorials used:

MASSIVE Unity Save System Tutorial!: https://www.youtube.com/watch?v=MotmZgCod9A

How to Spawn Objects in Unity [Using Instantiate]: https://www.youtube.com/watch?v=aBzpvUXibw0&t=8s

Adding Materials and Textures to Game Objects in Unity 6: https://www.youtube.com/watch?v=RnKkTW8R4A4&t=128s

Persistent data: How to save your game states and settings: https://unity.com/blog/games/persistent-data-how-to-save-your-game-states-and-settings

EVERYTHING you need to know to SAVE and LOAD your game in Unity: https://www.youtube.com/watch?v=1mf730eb5Wo

Placing and activating TRAPS in Unity 2020: https://www.youtube.com/watch?v=L2qJNZr2A_I

Documentation/ScriptReference/Random.Range: https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Random.Range.html

FULL 3D ENEMY AI in 6 MINUTES! || Unity Tutorial: https://www.youtube.com/watch?v=UjkSFoLxesw

How to Fix Pink Assets from Unity Asset Store in Unity 6: https://www.youtube.com/watch?v=YLfcRwrER4A&t=322s

How to make something spin in Unity | Unity 3D tutorial: https://www.youtube.com/watch?v=K3Ap2_beGnE

State Machine in Unity 3D - Tutorial for Beginners: https://www.youtube.com/watch?v=7QIcd6_TTys
*/


using UnityEngine;
using UnityEngine.AI;

public class Hazard : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 3f;
    public int damage = 1;
    public int health = 3;

    [Header("Combat")]
    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    [Header("Roaming")]
    public float roamRadius = 5f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    [Header("Visual Feedback")]
    public GameObject hitEffectPrefab;
    public GameObject goldCoinPrefab;

    private PlayerHealth playerHealth;

    private NavMeshAgent agent;
    private Transform player;

    private float waitTimer;
    private float attackTimer;
    private bool waiting;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("Hazard needs a NavMeshAgent!", this);
            enabled = false;
            return;
        }

        agent.speed = speed;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Hazard could not find an object tagged Player!");
        }
    }

    private void Start()
    {
        if (agent.isOnNavMesh && player != null)
            PickNewDestination();
    }

    private void Update()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        agent.speed = speed;
        attackTimer -= Time.deltaTime;

        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // Player is close enough to attack.
            if (distance <= attackRange)
            {
                waiting = false;
                agent.isStopped = true;
                agent.ResetPath();

                Vector3 lookDirection = player.position - transform.position;
                lookDirection.y = 0f;

                if (lookDirection.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.LookRotation(lookDirection);
                }

                if (attackTimer <= 0f)
                {
                    PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

                    if (playerHealth == null)
                    {
                        playerHealth = player.GetComponentInParent<PlayerHealth>();
                    }

                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(damage);
                        Debug.Log("Hazard attacked the player!");
                    }
                    else
                    {
                        Debug.LogWarning("PlayerHealth component not found on player.");
                    }

                    attackTimer = attackCooldown;
                }

                return;
            }

            // Chase the player when detected.
            if (distance <= detectionRange)
            {
                waiting = false;
                agent.isStopped = false;
                agent.SetDestination(player.position);
                return;
            }
        }

        // Resume roaming when the player is out of range.
        agent.isStopped = false;

        if (waiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                waiting = false;
                PickNewDestination();
            }

            return;
        }

        if (!agent.pathPending && agent.hasPath && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            agent.ResetPath();
            waiting = true;
            waitTimer = Random.Range(minWaitTime, maxWaitTime);
        }
        else if (!agent.pathPending && !agent.hasPath)
        {
            waiting = true;
            waitTimer = 1f;
        }
    }

    private void PickNewDestination()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        for (int i = 0; i < 20; i++)
        {
            Vector3 randomPoint = transform.position +
                new Vector3(Random.Range(-roamRadius, roamRadius), 0f, Random.Range(-roamRadius, roamRadius));

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, agent.areaMask))
            {
                NavMeshPath path = new NavMeshPath();

                if (agent.CalculatePath(hit.position, path) &&
                    path.status == NavMeshPathStatus.PathComplete)
                {
                    if (agent.SetDestination(hit.position))
                    {
                        waiting = false;
                        return;
                    }
                }
            }
        }

        waiting = true;
        waitTimer = 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Keep this if you want contact to destroy the enemy.
        // Actual attack damage is handled by Update().
        if (other.CompareTag("Player"))
        {
            // Damage is handled by the attack cooldown above.
            // Do not destroy the enemy here.
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab,transform.position, Quaternion.identity);
        }

        Debug.Log($"Enemy has {health} health left");

        if (health <= 0)
        {
            if (GameManager.Instance != null && GameManager.Instance.hasVampiricUpgrades)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

                if (playerObject != null)
                {
                    PlayerHealth playerHealth = playerObject.GetComponent<PlayerHealth>();

                    if (playerHealth != null)
                    {
                        playerHealth.HealPercent(GameManager.Instance.vampiricHealAmount);
                    }
                }
            }

            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab,transform.position,Quaternion.identity);
            }

            if (goldCoinPrefab != null)
            {
                Vector3 spawnPos = transform.position + Vector3.up * 0.5f;

                GameObject coin = Instantiate(goldCoinPrefab, spawnPos,Quaternion.Euler(90f, 0f, 0f));

                Rigidbody rb = coin.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    Vector3 randomForce =Random.insideUnitSphere * 2f + Vector3.up * 3f;

                    rb.AddForce(randomForce, ForceMode.Impulse);
                }
            }

            Destroy(gameObject);
        }
    }
}

