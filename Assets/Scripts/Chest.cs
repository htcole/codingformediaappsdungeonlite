using UnityEngine;
using UnityEngine.InputSystem;
public class Chest : MonoBehaviour
{
    [Header("Coins")]
    public int baseCoins = 20;
    public int extraCoinsPerRun = 10;

    [Header("Interaction")]
    public float interactRange = 2.5f;

    private Transform player;
    private bool opened = false;

    void Update()
    {
        if (opened || Keyboard.current == null) return;
        if (!Keyboard.current.eKey.wasPressedThisFrame) return;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player == null) return;
        if (Vector3.Distance(transform.position, player.position) > interactRange) return;

        Open();
    }

    void Open()
    {
        opened = true;

        int runsSoFar = Mathf.Max(0, DungeonGenerator.runCount - 1);
        int coins = baseCoins + extraCoinsPerRun * runsSoFar;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerGold += coins;
            Debug.Log("Chest opened: " + coins + " gold. Current Gold: " + GameManager.Instance.playerGold);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}