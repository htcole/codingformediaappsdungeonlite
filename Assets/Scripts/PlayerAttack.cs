using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionRange = 2f;

    void Update()
    {
        if (Keyboard.current == null) return;

        // Press 'E' to interact / strike nearby hazards
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            InteractWithNearbyHazards();
        }
    }

    void InteractWithNearbyHazards()
    {
        // Find all colliders within the interaction range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);

        foreach (Collider hitCollider in hitColliders)
        {
            Hazard hazard = hitCollider.GetComponent<Hazard>();
            if (hazard != null)
            {
                // Deal damage to the hazard when 'E' is pressed nearby
                hazard.TakeDamage(1);
                
                // Optional: break after hitting one target per keypress, or hit all in range
                break; 
            }
        }
    }

    // Optional: Draw a wire sphere in the editor so you can see your interaction range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}