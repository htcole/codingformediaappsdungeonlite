using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance so any script can access the GameManager easily
    public static GameManager Instance;

    [Header("Run Progression")]
    public int currentLevel = 1; // Increases every time you beat a boss/treasure room
    public int playerGold = 0;   // Currency carried across runs

    void Awake()
    {
        // Ensure only one GameManager exists across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this object alive when restarting scenes!
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this when you want to reset after death (keeps gold/meta-progression if desired)
    public void ResetRun()
    {
        currentLevel = 1;
    }
}