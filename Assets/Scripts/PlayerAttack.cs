using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;
    public int attackDamage = 1;

    [Header("Block Settings")]
    [Tooltip("True while the right mouse button is held.")]
    public bool isBlocking;

    [Header("Block Knockback")]
    public float blockRange = 1.5f;
    public float knockbackForce = 0.2f;
    public float knockbackCooldown = 0.3f;

    private float nextKnockbackTime;

    void Update()
    {
        if (Mouse.current == null) return;

        // Hold right click to block
        isBlocking = Mouse.current.rightButton.isPressed;

        if (isBlocking)
        {
            // Push nearby enemies away while blocking
            BounceEnemies();
        }
        else if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Left click to attack (can't attack while blocking)
            Attack();
        }
    }

    void Attack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider hitCollider in hitColliders)
        {
            Hazard hazard = hitCollider.GetComponent<Hazard>();
            if (hazard != null)
            {
                hazard.TakeDamage(attackDamage);
                break; // hit one target per click; remove to hit everything in range
            }
        }
    }

    void BounceEnemies()
    {
        if (Time.time < nextKnockbackTime) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, blockRange);
        bool pushedAny = false;

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Rigidbody rb = hit.attachedRigidbody;
            if (rb == null) continue;

            // Push horizontally, directly away from the player
            Vector3 dir = hit.transform.position - transform.position;
            dir.y = 0f;
            dir.Normalize();

            rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
            pushedAny = true;
        }

        if (pushedAny) nextKnockbackTime = Time.time + knockbackCooldown;
    }

    // Draw ranges in the editor: yellow = attack, cyan = block knockback
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, blockRange);
    }
}
