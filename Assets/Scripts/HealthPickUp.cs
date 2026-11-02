using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [Range(0f, 1f)] // Creates a nice slider in the Inspector from 0.0 to 1.0 (0% to 100%)
    public int healamount = 2; // Default to 25%
    private PlayerHealth playerHealth;
    public GameObject goldCoinPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Call the percentage heal method
                playerHealth.HealPercent(healamount);
            }

            Destroy(gameObject);
        }
    }
    private void Start() // Will delete and replace the health pick up object with coin(s) if player is on max health
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth.currentHealth >= playerHealth.maxHealth)
        {
            int randomNumber = Random.Range(1, 4);

            for (int i = 0; i < randomNumber; i++)
            {
                Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
                GameObject coin = Instantiate(goldCoinPrefab, spawnPos, Quaternion.Euler(90, 0, 0));
            }

            Destroy(gameObject);
        }
    }
}