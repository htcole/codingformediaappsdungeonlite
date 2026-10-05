using UnityEngine;
// Required namespace for Unity's new Input System
using UnityEngine.InputSystem;

public class UpgradeStation : MonoBehaviour
{
    public enum UpgradeType { MaxHealth, VampiricLeech }

    [Header("Shop Configuration")]
    public UpgradeType upgradeType;
    public int upgradeCost = 50;
    public string upgradeName = "Health Boost (+25 HP)";

    [Header("Interaction Settings")]
    public float interactionDistance = 3.0f;

    private Transform playerTransform;
    private bool hasNotifiedPlayer = false;

    void Start()
    {
        FindPlayer();
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= interactionDistance)
        {
            if (!hasNotifiedPlayer)
            {
                Debug.Log($"Near {upgradeName}! Press 'E' to buy for {upgradeCost} Gold.");
                hasNotifiedPlayer = true;
            }

            // --- UPDATED FOR NEW INPUT SYSTEM ---
            // Checks if the 'E' key was pressed this frame using the new Input System
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                TryBuyUpgrade();
            }
            // ------------------------------------
        }
        else
        {
            hasNotifiedPlayer = false;
        }
    }

    void TryBuyUpgrade()
    {
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.playerGold >= upgradeCost)
        {
            PlayerHealth playerHealth = playerTransform != null ? playerTransform.GetComponent<PlayerHealth>() : null;

            switch (upgradeType)
            {
                case UpgradeType.MaxHealth:
                    GameManager.Instance.playerGold -= upgradeCost;
                    GameManager.Instance.extraMaxHealthPurchased += 25; // Safely stored in persistent GameManager

                    // Instantly apply it to the current player if they are right there
                    if (playerHealth != null)
                    {
                        playerHealth.maxHealth = playerHealth.baseMaxHealth + GameManager.Instance.extraMaxHealthPurchased;
                        playerHealth.currentHealth += 25; // Heal them for the bonus amount
                    }

                    Debug.Log($"SUCCESS: Purchased Max Health! New Total Max: {GameManager.Instance.extraMaxHealthPurchased + 100}");
                    Destroy(gameObject);
                    break;

                case UpgradeType.VampiricLeech:
                    if (GameManager.Instance.hasVampiricUpgrades)
                    {
                        Debug.Log("You already own Vampiric Leech!");
                        return;
                    }

                    GameManager.Instance.playerGold -= upgradeCost;
                    GameManager.Instance.hasVampiricUpgrades = true;
                    Debug.Log("SUCCESS: Purchased Vampiric Leech! You now heal on hazard kills.");
                    Destroy(gameObject);
                    break;
            }
        }
        else
        {
            Debug.Log($"Not enough gold! You have {GameManager.Instance.playerGold} gold, but need {upgradeCost}.");
        }
    }
}