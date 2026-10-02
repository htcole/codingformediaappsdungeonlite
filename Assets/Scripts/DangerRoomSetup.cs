using UnityEngine;

public class DangerRoomSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("Drag your Hazard prefab here.")]
    public GameObject hazardPrefab;

    [Tooltip("Array of possible spawn points inside this room.")]
    public Transform[] spawnPoints;

    [Header("Difficulty Scaling")]
    [Tooltip("Base number of hazards spawned at Level 1.")]
    public int baseHazardCount = 1;

    [Tooltip("Speed range for the hazards.")]
    public float minSpeed = 2f;
    public float maxSpeed = 5f;

    void Start()
    {
        if (hazardPrefab == null) return;

        // 1. Get the current level from GameManager safely
        int currentLevel = 1;
        if (GameManager.Instance != null)
        {
            currentLevel = GameManager.Instance.currentLevel;
        }

        // 2. Increase hazard count as the level goes up (e.g., Level 1 = 1, Level 2 = 2, etc.)
        int hazardsToSpawn = baseHazardCount + (currentLevel - 1);

        // 3. Spawn the calculated number of hazards
        for (int i = 0; i < hazardsToSpawn; i++)
        {
            // Pick a random spawn point if available, otherwise spawn at the room center
            Transform spawnPoint = spawnPoints.Length > 0
                ? spawnPoints[Random.Range(0, spawnPoints.Length)]
                : transform;

            GameObject hazardObj = Instantiate(hazardPrefab, spawnPoint.position, Quaternion.identity, transform);

            // 4. Randomize speed and scale it slightly with higher levels
            Hazard hazardScript = hazardObj.GetComponent<Hazard>();
            if (hazardScript != null)
            {
                float scaledMin = minSpeed + (currentLevel * 0.2f);
                float scaledMax = maxSpeed + (currentLevel * 0.5f);
                hazardScript.speed = Random.Range(scaledMin, scaledMax);
            }
        }
    }
}