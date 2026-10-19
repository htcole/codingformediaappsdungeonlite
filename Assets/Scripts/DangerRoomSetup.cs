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
            Light light = hazardObj.GetComponent<Light>();
            MeshRenderer meshRenderer = hazardObj.GetComponent<MeshRenderer>();
            ParticleSystem particleSystem = hazardObj.GetComponent<ParticleSystem>();
            if (hazardScript != null)
            {
                float scaledMin = minSpeed + (currentLevel * 0.2f);
                float scaledMax = maxSpeed + (currentLevel * 0.5f);
                int randomNumber = Random.Range(1, 11);
                if (randomNumber < 7 && randomNumber >= 5) // Speedy
                {
                    hazardScript.speed = Random.Range(scaledMin + 5f, scaledMax + 5f);
                    light.color = Color.blue;
                    meshRenderer.material.color = Color.blue;
                    var emit = particleSystem.emission;
                    emit.enabled = true;
                }
                else if (randomNumber < 9 && randomNumber >= 7) // Tanky
                {
                    hazardScript.speed = Random.Range(scaledMin - 0.5f, scaledMax - 0.5f);
                    hazardScript.health += 1;
                    light.color = Color.green;
                    meshRenderer.material.color = Color.green;
                    var emit = particleSystem.emission;
                    emit.enabled = true;
                }
                else if (randomNumber < 11 && randomNumber >= 9) // Fighter
                {
                    hazardScript.speed = Random.Range(scaledMin + 0.5f, scaledMax + 0.5f);
                    hazardScript.damage += 1;
                    light.color = Color.orange;
                    meshRenderer.material.color = Color.orange;
                    var emit = particleSystem.emission;
                    emit.enabled = true;
                }
                else
                {
                    hazardScript.speed = Random.Range(scaledMin, scaledMax);
                }
            }
        }
    }
}