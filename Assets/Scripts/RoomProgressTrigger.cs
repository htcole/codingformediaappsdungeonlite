using UnityEngine;

public class RoomProgressTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;

            // Tell the Dungeon Manager to spawn the next room in line using the modern method
            Object.FindAnyObjectByType<DungeonGenerator>()?.SpawnNextRoomInSequence();

            // Disable this trigger so it only happens once
            gameObject.SetActive(false);
        }
    }
}