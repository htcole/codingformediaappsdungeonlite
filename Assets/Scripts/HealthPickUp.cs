using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [Range(0f, 1f)] // Creates a nice slider in the Inspector from 0.0 to 1.0 (0% to 100%)
    public float healPercentage = 0.25f; // Default to 25%

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Call the percentage heal method
                playerHealth.HealPercent(healPercentage);
            }

            Destroy(gameObject);
        }
    }
}