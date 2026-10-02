using UnityEngine;
using UnityEngine.SceneManagement;

public class TreasureRoomTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;

            // 1. Increase the level count in your persistent GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.currentLevel++;
                Debug.Log("Level Complete! Advancing to Level: " + GameManager.Instance.currentLevel);
            }

            // 2. Reload the scene to generate a fresh dungeon with scaled difficulty
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}