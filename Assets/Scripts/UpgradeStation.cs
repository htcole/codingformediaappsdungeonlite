/*Tutorials used:

Tutorials used:

MASSIVE Unity Save System Tutorial!: https://www.youtube.com/watch?v=MotmZgCod9A

How to Spawn Objects in Unity [Using Instantiate]: https://www.youtube.com/watch?v=aBzpvUXibw0&t=8s

Adding Materials and Textures to Game Objects in Unity 6: https://www.youtube.com/watch?v=RnKkTW8R4A4&t=128s

Persistent data: How to save your game states and settings: https://unity.com/blog/games/persistent-data-how-to-save-your-game-states-and-settings

EVERYTHING you need to know to SAVE and LOAD your game in Unity: https://www.youtube.com/watch?v=1mf730eb5Wo

Placing and activating TRAPS in Unity 2020: https://www.youtube.com/watch?v=L2qJNZr2A_I

Documentation/ScriptReference/Random.Range: https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Random.Range.html

FULL 3D ENEMY AI in 6 MINUTES! || Unity Tutorial: https://www.youtube.com/watch?v=UjkSFoLxesw

How to Fix Pink Assets from Unity Asset Store in Unity 6: https://www.youtube.com/watch?v=YLfcRwrER4A&t=322s

How to make something spin in Unity | Unity 3D tutorial: https://www.youtube.com/watch?v=K3Ap2_beGnE

State Machine in Unity 3D - Tutorial for Beginners: https://www.youtube.com/watch?v=7QIcd6_TTys
*/


using TMPro;
using UnityEngine;
// Required namespace for Unity's new Input System
using UnityEngine.InputSystem;

public class UpgradeStation : MonoBehaviour
{
    public enum UpgradeType { MaxHealth, VampiricLeech, AttackPower, Speed}

    [Header("Shop Configuration")]
    public UpgradeType upgradeType;
    public int upgradeCost = 50;
    public string upgradeName = "Health Boost (+25 HP)";
    public int damage = 1;
    public float speed = 12f;
    public TextMeshProUGUI Attack_Power;

    [Header("Interaction Settings")]
    public float interactionDistance = 3.0f;

    private Transform playerTransform;
    private bool hasNotifiedPlayer = false;

    public static object Instance { get; internal set; }

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

                case UpgradeType.AttackPower:
                    GameManager.Instance.playerGold -= upgradeCost;
                    GameManager.Instance.ExtraAttackPower += 1;
                    damage = damage + 1;
                    Debug.Log($"SUCCESS: Purchased Extra Attack! New Total Max: {GameManager.Instance.ExtraAttackPower + 1}");
                    Destroy(gameObject);
                    break;

                case UpgradeType.Speed:
                    GameManager.Instance.playerGold -= upgradeCost;
                    GameManager.Instance.Speed += 2f;
                    speed = speed + 2f;
                    Debug.Log($"SUCCESS: Purchased Extra Speed! New Total Max: {GameManager.Instance.Speed + 1}");
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