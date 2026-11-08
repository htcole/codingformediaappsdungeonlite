using UnityEngine;
using UnityEngine.Events;

// Put this on any object with a trigger collider.
// When the player walks into it, they get teleported to "destination".
// DungeonGenerator adds this automatically to the exit door it spawns.
public class LayerExit : MonoBehaviour
{
    [HideInInspector] public Vector3 destination;

    // Set to false for an exit that shouldn't move the player
    [HideInInspector] public bool teleport = true;

    // Fires when the player walks in, before any teleport
    public UnityEvent onEntered = new UnityEvent();

    void OnTriggerEnter(Collider other)
    {
        // Debug output: remove these Debug.Log lines once the door works again
        Debug.Log("LayerExit: touched by '" + other.name + "' (tag: " + other.tag + ", layer: " + LayerMask.LayerToName(other.gameObject.layer) + ")");

        if (!other.CompareTag("Player"))
        {
            Debug.Log("LayerExit: ignored, it isn't tagged 'Player'.");
            return;
        }

        onEntered.Invoke();

        if (!teleport)
        {
            Debug.Log("LayerExit: not teleporting (this door starts the next run instead).");
            return;
        }

        Debug.Log("LayerExit: teleporting player to " + destination);

        // CharacterControllers fight against direct position changes, so toggle it off briefly
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        other.transform.position = destination;

        if (cc != null) cc.enabled = true;
    }
}
