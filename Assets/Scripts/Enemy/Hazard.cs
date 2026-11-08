using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 3f;
    public int damage = 1;
    public int health = 100; // Added health so it can be destroyed by the player

    [Header("Scaling (per generation cleared)")]
    [Tooltip("Extra health per generation cleared. 0.2 = +20% each time, with no limit.")]
    public float healthPerRun = 0.2f;
    [Tooltip("Extra contact damage per generation cleared. 0.15 = +15% each time, with no limit.")]
    public float damagePerRun = 0.15f;

    [Header("Boss")]
    [Tooltip("Tick this on the boss. Killing it makes every enemy hit harder and drop an extra coin.")]
    public bool isBoss = false;

    // How many bosses have been beaten. Each one adds +1 damage and +1 coin to every enemy.
    public static int bossesDefeated = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetBossCount()
    {
        bossesDefeated = 0;
    }

    [Header("Visual Feedback")]
    // Drag your HitSpark prefab here in the inspector
    public GameObject hitEffectPrefab;
    // Drag you GoldCoin prefab here in the inspector
    public GameObject goldCoinPrefab;

    private Transform player;
    private int scaledDamage;

    void Awake()
    {
        // The first dungeon is generation 1, so nothing has been cleared yet
        int generationsCleared = Mathf.Max(0, DungeonGenerator.runCount - 1);

        health = Mathf.RoundToInt(health * (1f + healthPerRun * generationsCleared));
        scaledDamage = Mathf.RoundToInt(damage * (1f + damagePerRun * generationsCleared));
    }

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    //Damage the player on contact
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(scaledDamage + bossesDefeated);
        }
    }

    // The function called by PlayerAttack (or TreasureRoomTrigger)
    public void TakeDamage(int amount)
    {
        health -= amount;

        // ---- Visual Feedback on Hit ----
        // Instantiate the hit effect at our current position/rotation
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
        // -------------------------------------

        if (health <= 0)
        {
            // ---- VAMPIRIC LEECH UPGRADE CHECK ----
            if (GameManager.Instance != null && GameManager.Instance.hasVampiricUpgrades)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    PlayerHealth playerHealth = playerObj.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        // Heal player using the value defined in GameManager
                        playerHealth.HealPercent(GameManager.Instance.vampiricHealAmount);
                        Debug.Log("Vampiric Leech triggered! Health stolen from enemy.");
                    }
                }
            }
            // --------------------------------------

            // Visual Effect on Death
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            // Spawn coin loot: 1 coin, plus 1 more for every boss beaten
            if (goldCoinPrefab != null)
            {
                int coinCount = 1 + bossesDefeated;

                for (int n = 0; n < coinCount; n++)
                {
                    Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
                    GameObject coin = Instantiate(goldCoinPrefab, spawnPos, Quaternion.Euler(90, 0, 0));

                    Rigidbody rb = coin.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 randomForce = Random.insideUnitSphere * 2f + Vector3.up * 3f;
                        rb.AddForce(randomForce, ForceMode.Impulse);
                    }
                }
            }

            // Beating the boss powers up every enemy from now on
            if (isBoss)
            {
                bossesDefeated++;
            }

            Destroy(gameObject);
        }
    }
}
