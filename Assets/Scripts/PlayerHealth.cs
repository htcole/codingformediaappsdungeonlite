using UnityEngine;
using UnityEngine.SceneManagement; // Required for restarting or loading scenes

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("The maximum health the player starts with.")]
    public int maxHealth = 10;
    public int currentHealth; // Tracks the player's health during gameplay

    [Header("UI References")]
    [Tooltip("Drag the Game Over Panel UI object here in the Inspector.")]
    public GameObject gameOverPanel;

    void Start()
    {
        // At the start of the game, set current health to maximum
        currentHealth = maxHealth;

        // Ensure the Game Over screen is hidden when the game begins
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

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log("Player healed! Current Health: " + currentHealth);
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