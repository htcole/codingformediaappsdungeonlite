using UnityEngine;
using UnityEngine.InputSystem;

public class HealthPotion : MonoBehaviour
{
    [Header("Potion")]
    public int healAmount = 10;

    [Header("Interaction")]
    public float interactRange = 2.5f;

    private Transform player;

    void Update()
    {
        if (Keyboard.current == null) return;
        if (!Keyboard.current.eKey.wasPressedThisFrame) return;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player == null) return;
        if (Vector3.Distance(transform.position, player.position) > interactRange) return;

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health == null) return;

        health.Heal(healAmount);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
