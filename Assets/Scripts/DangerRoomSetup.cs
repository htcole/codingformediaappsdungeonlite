/*Tutorials used:

Tutorials used:

MASSIVE Unity Save System Tutorial!: https://www.youtube.com/watch?v=MotmZgCod9A

How to Spawn Objects in Unity [Using Instantiate]: https://www.youtube.com/watch?v=aBzpvUXibw0&t=8s

Adding Materials and Textures to Game Objects in Unity 6: https://www.youtube.com/watch?v=RnKkTW8R4A4&t=128s

Persistent data: How to save your game states and settings: https://unity.com/blog/games/persistent-data-how-to-save-your-game-states-and-settings

EVERYTHING you need to know to SAVE and LOAD your game in Unity: https://www.youtube.com/watch?v=1mf730eb5Wo

Placing and activating TRAPS in Unity 2020: https://www.youtube.com/watch?v=L2qJNZr2A_I

Documentation/ScriptReference/Random.Range: https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Random.Range.html

FULL 3D ENEMY AI in 6 MINUTES! || Unity Tutorial: https://www.youtube.com/watch?v=UjkSFoLxesw

How to Fix Pink Assets from Unity Asset Store in Unity 6: https://www.youtube.com/watch?v=YLfcRwrER4A&t=322s

How to make something spin in Unity | Unity 3D tutorial: https://www.youtube.com/watch?v=K3Ap2_beGnE

State Machine in Unity 3D - Tutorial for Beginners: https://www.youtube.com/watch?v=7QIcd6_TTys
*/
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

       // Safety check: If spawnPoints is null or empty, fall back gracefully
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("SpawnPoints array is unassigned on " + gameObject.name + "! Spawning at room center instead.");
        }

        for (int i = 0; i < hazardsToSpawn; i++)
        {
            // Pick a random spawn point if assigned and has items, otherwise spawn at room center (transform)
            Transform spawnPoint = (spawnPoints != null && spawnPoints.Length > 0)
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