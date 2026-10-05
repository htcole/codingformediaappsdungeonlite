using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 3f;
    public int damage = 1;
    public int health = 2; // Added health so it can be destroyed by the player

    [Header("Visual Feedback")]
    // Drag your HitSpark prefab here in the inspector
    public GameObject hitEffectPrefab;
    // Drag you GoldCoin prefab here in the inspector
    public GameObject goldCoinPrefab;

    private Transform player;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        //Only move if speed is set and player exists
        if (speed > 0 && player != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    //Damage the player on contact
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
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

            // Spawn coin loot
            if (goldCoinPrefab != null)
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

            Destroy(gameObject);
        }
    }
}