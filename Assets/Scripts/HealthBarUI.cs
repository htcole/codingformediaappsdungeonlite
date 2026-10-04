using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Required for Slider

public class HealthBarUI : MonoBehaviour
{
    private Slider healthSlider;
    private PlayerHealth playerHealth;

    void Start()
    {
        healthSlider = GetComponent<Slider>();
        // Start looking for the player
        StartCoroutine(FindPlayerRoutine());
    }

    IEnumerator FindPlayerRoutine()
    {
        // Keep checking every frame until the player object with the tag "Player" exists
        while (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
            }
            yield return null; // Wait for the next frame
        }

        // Once found, initialize the slider values immediately
        if (healthSlider != null && playerHealth != null)
        {
            healthSlider.maxValue = playerHealth.maxHealth;
            healthSlider.value = playerHealth.currentHealth;
        }
    }

    void Update()
    {
        // Continuously update the slider to match player health
        if (playerHealth != null && healthSlider != null)
        {
            healthSlider.maxValue = playerHealth.maxHealth;
            healthSlider.value = playerHealth.currentHealth;
        }
    }
}