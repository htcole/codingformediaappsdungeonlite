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

    public void SpawnNextRoomInSequence()
    {
        if (roomQueue.Count == 0) return;

        RoomType nextType = roomQueue.Dequeue();
        GameObject prefabToSpawn = null;

        switch (nextType)
        {
            case RoomType.Danger:
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