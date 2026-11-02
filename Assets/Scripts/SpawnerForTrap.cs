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

public class Spawn : MonoBehaviour
{
    [Header("Trap Settings")]
    public GameObject enemyPrefab;
    [Range(0, 100)]
    public int enemySpawnChance = 60;

    [Header("Treasure Settings")]
    public GameObject GoldCoin;
    public int minCoins = 3;
    public int maxCoins = 6;
    public float coinSpawnRadius = 1f;
    public float coinLaunchForce = 3f;

    public int minEnemies = 1;
    public int maxEnemies = 4;
    public float enemySpawnRadius = 3f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {

        if (hasTriggered || !other.CompareTag("Player"))
            return;

        Debug.Log("Triggered Spawn object: " + gameObject.name);
        Debug.Log("GoldCoin reference: " + (GoldCoin != null ? GoldCoin.name : "NULL"));

        hasTriggered = true;

        int roll = Random.Range(1, 101);

        if (roll <= enemySpawnChance)
        {
            SpawnEnemies();
            Debug.Log("Trap triggered! Enemy spawned.");
        }
        else
        {
            SpawnCoins();
            Debug.Log("Treasure triggered! Extra coins spawned.");
        }

        Collider triggerCollider = GetComponent<Collider>();

        if (triggerCollider != null)
            triggerCollider.enabled = false;
    }

    private void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is missing!", this);
            return;
        }

        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);

        Debug.Log("TRAP SELECTED! Spawning " + enemyCount + " enemies.");

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 basePosition = transform.position + transform.forward * 10f;

            Vector2 randomOffset = Random.insideUnitCircle * enemySpawnRadius;

            Vector3 spawnPosition = basePosition + new Vector3(randomOffset.x, 0f,randomOffset.y);

            // Find a valid NavMesh position.
            if (enemyPrefab.GetComponentInChildren<NavMeshAgent>() != null)
            {
                if (NavMesh.SamplePosition(spawnPosition,out NavMeshHit hit,5f,NavMesh.AllAreas))
                {
                    spawnPosition = hit.position;
                }
                else
                {
                    Debug.LogWarning("Could not find NavMesh position for enemy " + (i + 1));
                    continue;
                }
            }

            Instantiate(enemyPrefab,spawnPosition,Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
            );

            Debug.Log("Enemy " + (i + 1) + " spawned at " + spawnPosition);
        }
    }

    private void SpawnCoins()
    {
        if (GoldCoin == null)
        {
            Debug.LogError("GoldCoin prefab is NOT assigned!", this);
            return;
        }

        int coinCount = Random.Range(minCoins, maxCoins + 1);

        Debug.Log("TREASURE SELECTED! Spawning " + coinCount + " coins.");

        for (int i = 0; i < coinCount; i++)
        {
            // Random horizontal position around the TrapSpawner
            Vector3 randomOffset = new Vector3(
                Random.Range(-coinSpawnRadius, coinSpawnRadius),
                0f,
                Random.Range(-coinSpawnRadius, coinSpawnRadius)
            );

            // Start raycast above the floor
            Vector3 rayStart = transform.position + randomOffset + Vector3.up * 5f;

            RaycastHit hit;

            if (Physics.Raycast(rayStart, Vector3.down, out hit, 15f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                // Spawn coin slightly above the ground
                Vector3 spawnPosition = hit.point + Vector3.up * 0.1f + Vector3.forward * 10f;

                GameObject coin = Instantiate(GoldCoin,spawnPosition,Quaternion.identity);

                Debug.Log("Coin spawned on ground at: " + spawnPosition);

                Rigidbody rb = coin.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
            else
            {
                Debug.LogWarning("Could not find ground beneath coin spawn position!");
            }
        }
    }
}
