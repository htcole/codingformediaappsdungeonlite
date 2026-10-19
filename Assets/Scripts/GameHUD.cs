using UnityEngine;
using TMPro;
using System.Collections; // Required for TextMeshPro

public class GameHUD : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI healthText;
    private PlayerHealth playerHealth;

    void Start()
    {
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
    }

    void Update()
    {
        // Continuously update the text to match the GameManager's gold value
        if (GameManager.Instance != null && goldText != null)
        {
            goldText.text = "Gold: " + GameManager.Instance.playerGold;
            speedText.text = "Speed: " + (GameManager.Instance.extraSpeedPurchased + 4);
            rangeText.text = "Range: " + (GameManager.Instance.extraWeaponRangePurchased + 2);
            levelText.text = "Level: " + GameManager.Instance.currentLevel;
            healthText.text = "" + playerHealth.currentHealth;
        }
    }
}