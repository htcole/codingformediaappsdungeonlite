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
    public int health = 2;
    public float minSpeed = 2f;
    public float maxSpeed = 5f;

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
        roomQueue.Enqueue(RoomType.Danger);
        roomQueue.Enqueue(RoomType.Safe);
        roomQueue.Enqueue(RoomType.Boss);
        roomQueue.Enqueue(RoomType.Boss);
        roomQueue.Enqueue(RoomType.Treasure);

        // 2. Spawn the initial Start Room
        Instantiate(original: startRoomPrefab, nextSpawnPos, Quaternion.identity);
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
                health = health + 1;
                minSpeed = minSpeed + 1f;
                maxSpeed = maxSpeed + 5f;
                break;
        }

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, nextSpawnPos, Quaternion.identity);
            nextSpawnPos += new Vector3(0, 0, roomSize);
        }
    }
    public void SpawnRoomsUpToPosition(Vector3 targetPosition)
    {
        while (roomQueue.Count > 0 && nextSpawnPos.z <= targetPosition.z)
        {
            SpawnNextRoomInSequence();
        }
        Debug.Log("Rooms spawned up to saved Z position: " + targetPosition.z);
    }
}
