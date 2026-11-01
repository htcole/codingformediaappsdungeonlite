using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Room Prefabs")]
    public GameObject startRoomPrefab;
    public GameObject[] dangerRoomPrefabs;
    public GameObject safeRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject treasureRoomPrefab;

    [Header("Generator Settings")]
    public float roomSize = 20f;
    private Vector3 nextSpawnPos = Vector3.zero;

    // A queue tracking what room types are coming up next
    private Queue<RoomType> roomQueue = new Queue<RoomType>();

    private enum RoomType { Danger, Safe, Boss, Treasure }

    void Start()
    {
        // 1. Build the exact sequence you want for this level loop
        roomQueue.Enqueue(RoomType.Danger);
        roomQueue.Enqueue(RoomType.Safe);
        roomQueue.Enqueue(RoomType.Danger);
        roomQueue.Enqueue(RoomType.Safe);
        roomQueue.Enqueue(RoomType.Boss);
        roomQueue.Enqueue(RoomType.Treasure);

        // 2. Spawn the initial Start Room
        Instantiate(startRoomPrefab, nextSpawnPos, Quaternion.identity);
        nextSpawnPos += new Vector3(0, 0, roomSize);
    }

    private void GenerateRandomSequence(int totalRoomsToGenerate)
    {
        // Get the total number of options in the RoomType enum
        int totalRoomTypes = System.Enum.GetValues(typeof(RoomType)).Length;

        // Loop for the designated length of your dungeon minus 1 (reserving the last spot for a boss)
        for (int i = 0; i < totalRoomsToGenerate - 1; i++)
        {
            // Pick a random room type (excluding Boss so it doesn't appear early)
            // Random.Range(0, 3) will pick 0 (Danger), 1 (Safe), or 2 (Boss - wait, let's fix the order or cap it)
            // To exclude Boss safely, let's cast a random integer from our safe options

            int randomIndex = Random.Range(0, 3); // Picks 0, 1, or 2 (Danger, Safe, Boss). Let's adjust logic:

            // Cleaner approach: Pick random among Danger, Safe, Treasure
            RoomType randomType = PickRandomNormalRoom();
            roomQueue.Enqueue(randomType);
        }

        // Always guarantee a Boss room at the very end of the sequence
        roomQueue.Enqueue(RoomType.Boss);
    }

    private RoomType PickRandomNormalRoom()
    {
        // Weighted or unweighted random selection
        float roll = Random.value;

        if (roll < 0.6f) return RoomType.Danger;      // 60% chance for a danger room
        if (roll < 0.85f) return RoomType.Safe;     // 15% chance for a safe room
        return RoomType.Treasure;                  
    }

    public void SpawnNextRoomInSequence()
    {
        if (roomQueue.Count == 0) return;

        RoomType nextType = roomQueue.Dequeue();
        GameObject prefabToSpawn = null;

        switch (nextType)
        {
            case RoomType.Danger:
                if (dangerRoomPrefabs.Length > 0)
                    prefabToSpawn = dangerRoomPrefabs[Random.Range(0, dangerRoomPrefabs.Length)];
                break;
            case RoomType.Safe:
                prefabToSpawn = safeRoomPrefab;
                break;
            case RoomType.Boss:
                prefabToSpawn = bossRoomPrefab;
                break;
            case RoomType.Treasure:
                prefabToSpawn = treasureRoomPrefab;
                break;
        }

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, nextSpawnPos, Quaternion.identity);
            nextSpawnPos += new Vector3(0, 0, roomSize);
        }
    }
}