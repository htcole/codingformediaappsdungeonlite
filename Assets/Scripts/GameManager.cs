using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance so any script can access the GameManager easily
    public static GameManager Instance;

    [Header("Run Progression")]
    public int currentLevel = 1; // Increases every time you beat a boss/treasure room
    public int playerGold = 0;   // Currency carried across runs
    public int extraMaxHealthPurchased = 0; // Purchased via health upgrade
    public bool hasVampiricUpgrades = false; // Purchased via leech upgrade
    public int extraSpeedPurchased = 0;
    public float extraWeaponRangePurchased = 0f;
    public int additionalUpgradeCost = 0;
    public int vampiricHealAmount = 1;
    public int currentHealthBeforeNextLevel = 10; // Saves player health so you don't get a full heal at the start of each level
    public string info = "";

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
        playerGold = 0;
        extraMaxHealthPurchased = 0;
        extraWeaponRangePurchased = 0f;
        hasVampiricUpgrades = false;
        extraSpeedPurchased = 0;
        additionalUpgradeCost = 0;
        currentHealthBeforeNextLevel = 10;
        info = "";
    }
}