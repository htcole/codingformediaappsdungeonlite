using UnityEngine;
using UnityEngine.SceneManagement; // Required for restarting or loading scenes

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("The maximum health the player starts with.")]
    public int baseMaxHealth = 10;
    [HideInInspector]
    public int maxHealth; // Calculated dynamically
    public int currentHealth; // Tracks the player's health during gameplay

    [Header("UI References")]
    [Tooltip("Drag the Game Over Panel UI object here in the Inspector.")]
    public GameObject gameOverPanel;

    void Start()
    {
        // Calculate max health: Base health + any meta-upgrades bought in GameManager
        maxHealth = baseMaxHealth;
        if (GameManager.Instance != null)
        {
            maxHealth += GameManager.Instance.extraMaxHealthPurchased;
        }

        // Set current health to the new total max health on spawn
        currentHealth = maxHealth;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    // Call this function whenever the player takes damage
    public void TakeDamage(int amount)
    {
        // Subtract the damage amount from current health
        currentHealth -= amount;
        Debug.Log("Player took damage! Current Health: " + currentHealth);

        // Check if health has dropped to zero or below
        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    // Heal by a percentage of max health (e.g., pass 0.25f for 25%)
    public void HealPercent(float percentage)
    {
        // Calculate the integer amount based on current max health
        int healAmount = Mathf.RoundToInt(maxHealth * percentage);

        currentHealth += healAmount;

        // Clamp so it never exceeds maxHealth
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log($"Healed by {percentage * 100}% ({healAmount} HP)! Current Health: {currentHealth}/{maxHealth}");
    }
    // Handles what happens when the player dies
    void GameOver()
    {
        // Show the Game Over UI panel if it's assigned
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Pause the game time so enemies and traps stop moving
        Time.timeScale = 0f;
    }

    // Linked to the Restart button in your UI to reset the game
    public void RestartGame()
    {
        // Unpause the game time before reloading the scene
        Time.timeScale = 1f;

        // Reloads the currently active scene from the beginning
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}