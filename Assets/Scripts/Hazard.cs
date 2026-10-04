using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 3f;
    public int damage = 1;
    public int health = 1; // Added health so it can be destroyed by the player

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
           // Instantiate the hit effect
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }
            // 2. Spawn the physical coin loot
            if (goldCoinPrefab != null)
            {
                // Spawn it slightly above the ground
                Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
                GameObject coin = Instantiate(goldCoinPrefab, spawnPos, Quaternion.Euler(90, 0, 0)); // Rotate upright if needed

                // Add a little "pop" force upwards/outwards
                Rigidbody rb = coin.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 randomForce = Random.insideUnitSphere * 2f + Vector3.up * 3f;
                    rb.AddForce(randomForce, ForceMode.Impulse);
                }
            }
            // ----------------------------------

            // Destroy the hazard
            Destroy(gameObject);
        }
    }
}