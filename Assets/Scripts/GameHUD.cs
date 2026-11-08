using UnityEngine;
using TMPro; // Required for TextMeshPro

public class GameHUD : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI goldText;

    void Update()
    {
        // Continuously update the text to match the GameManager's gold value
        if (GameManager.Instance != null && goldText != null)
        {
            goldText.text = "Gold: " + GameManager.Instance.playerGold;
        }
    }
}