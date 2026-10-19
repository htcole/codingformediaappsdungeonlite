using UnityEngine;
// Required namespace for Unity's new Input System
using UnityEngine.InputSystem;

public class UpgradeStation : MonoBehaviour
{
    public enum UpgradeType { MaxHealth, VampiricLeech, Speed, WeaponRange }

    [Header("Shop Configuration")]
    public UpgradeType upgradeType;
    public int upgradeCost = 50;
    private int updateCostIncrease = 35;
    public string upgradeName = "Health Boost (+5 HP)";

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
                Debug.Log($"Near {upgradeName}! Press 'E' to buy for {GameManager.Instance.additionalUpgradeCost + upgradeCost} Gold.");
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

        if (GameManager.Instance.playerGold >= GameManager.Instance.additionalUpgradeCost + upgradeCost)
        {
            PlayerHealth playerHealth = playerTransform != null ? playerTransform.GetComponent<PlayerHealth>() : null;
            SimplePlayerMovement simplePlayerMovement = playerTransform != null ? playerTransform.GetComponent<SimplePlayerMovement>() : null;
            PlayerAttack playerAttack = playerTransform != null ? playerTransform.GetComponent<PlayerAttack>() : null;

            switch (upgradeType)
            {
                case UpgradeType.MaxHealth:
                    GameManager.Instance.playerGold -= GameManager.Instance.additionalUpgradeCost + upgradeCost;
                    GameManager.Instance.extraMaxHealthPurchased += 5; // Safely stored in persistent GameManager

                    // Instantly apply it to the current player if they are right there
                    if (playerHealth != null)
                    {
                        playerHealth.maxHealth = playerHealth.baseMaxHealth + GameManager.Instance.extraMaxHealthPurchased;
                        playerHealth.currentHealth += 5; // Heal them for the bonus amount
                    }

                    GameManager.Instance.additionalUpgradeCost += updateCostIncrease;
                    Debug.Log($"SUCCESS: Purchased Max Health! New Total Max: {GameManager.Instance.extraMaxHealthPurchased + 100}");
                    Destroy(gameObject);
                    break;

                case UpgradeType.VampiricLeech:
                    if (GameManager.Instance.hasVampiricUpgrades)
                    {
                        Debug.Log("You already own Vampiric Leech!");
                        return;
                    }

                    GameManager.Instance.playerGold -= GameManager.Instance.additionalUpgradeCost + upgradeCost;
                    GameManager.Instance.hasVampiricUpgrades = true;
                    GameManager.Instance.additionalUpgradeCost += updateCostIncrease;
                    Debug.Log("SUCCESS: Purchased Vampiric Leech! You now heal on hazard kills.");
                    Destroy(gameObject);
                    break;

                case UpgradeType.Speed:
                    GameManager.Instance.playerGold -= GameManager.Instance.additionalUpgradeCost + upgradeCost;
                    GameManager.Instance.extraSpeedPurchased += 1;

                    simplePlayerMovement.speed += 1;

                    GameManager.Instance.additionalUpgradeCost += updateCostIncrease;
                    Debug.Log($"SUCCESS: Purchased Speed! New Total Max: {GameManager.Instance.extraSpeedPurchased + 4}");
                    Destroy(gameObject);
                    break;

                case UpgradeType.WeaponRange:
                    GameManager.Instance.playerGold -= GameManager.Instance.additionalUpgradeCost + upgradeCost;
                    GameManager.Instance.extraWeaponRangePurchased += 0.2f;

                    GameManager.Instance.additionalUpgradeCost += updateCostIncrease;
                    Debug.Log($"SUCCESS: Purchased Range! New Total Max: {GameManager.Instance.extraWeaponRangePurchased + playerAttack.interactionRange}");
                    Destroy(gameObject);
                    break;
            }
        }
        else
        {
            Debug.Log($"Not enough gold! You have {GameManager.Instance.playerGold} gold, but need {GameManager.Instance.additionalUpgradeCost + upgradeCost}.");
        }
    }
}