using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarColorChange : MonoBehaviour
{
    private Image healthBar;
    private PlayerHealth playerHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthBar = GetComponent<Image>();
        // Start looking for the player
        StartCoroutine(FindPlayerRoutine());
    }

    IEnumerator FindPlayerRoutine()
    {
        while (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
            }
            yield return null;
        }

        if (playerHealth != null && healthBar != null)
        {
            healthBar.color = Color.green;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth != null && healthBar != null)
        {
            if (playerHealth.currentHealth >= playerHealth.maxHealth * 0.5)
            {
                healthBar.color = Color.green;
            }
            else if (playerHealth.currentHealth < playerHealth.maxHealth * 0.5 && playerHealth.currentHealth > playerHealth.maxHealth * 0.2)
            {
                healthBar.color = Color.yellow;
            }
            else
            {
                healthBar.color = Color.red;
            }
        }
    }
}
