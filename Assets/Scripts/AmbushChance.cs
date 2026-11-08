using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Each time the player walks into a room for the first time, there's a chance an enemy
// ambushes them there. The chance starts at 0% on the first generation and rises every
// generation until it reaches the maximum. Put this on one empty GameObject in the scene.
public class AmbushChance : MonoBehaviour
{
    [Header("Chance (per new room)")]
    [Tooltip("Chance added for every generation cleared. 0.05 = +5%.")]
    [Range(0f, 1f)] public float chancePerGeneration = 0.05f;
    [Tooltip("The chance never goes above this. 0.75 = 75%.")]
    [Range(0f, 1f)] public float maxChance = 0.75f;

    [Header("Ambush")]
    [Tooltip("Enemies that can spawn in the room the player just entered.")]
    public GameObject[] ambushEnemies;
    [Tooltip("The enemy tries to spawn at least this far from the player.")]
    public float ambushMinDistance = 4f;

    // 0% on generation 1, then +5% each generation, up to the maximum
    public float CurrentChance
    {
        get
        {
            int generationsCleared = Mathf.Max(0, DungeonGenerator.runCount - 1);
            return Mathf.Min(maxChance, chancePerGeneration * generationsCleared);
        }
    }

    private DungeonGenerator dungeon;
    private GameObject player;
    private int lastRun = -1;
    private Transform currentRoom;

    private readonly Dictionary<Transform, Bounds> roomBounds = new Dictionary<Transform, Bounds>();
    private readonly HashSet<Transform> visited = new HashSet<Transform>();

    void Update()
    {
        if (dungeon == null)
        {
            dungeon = FindFirstObjectByType<DungeonGenerator>();
            if (dungeon == null) return;
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
        }

        // A new dungeon was generated: measure its rooms and forget the old ones.
        // The room the player spawns in doesn't count as entered.
        if (DungeonGenerator.runCount != lastRun)
        {
            lastRun = DungeonGenerator.runCount;

            RebuildRooms();
            visited.Clear();

            currentRoom = FindRoomAt(player.transform.position);
            if (currentRoom != null) visited.Add(currentRoom);
            return;
        }

        Vector3 pos = player.transform.position;

        // Still in the same room: nothing to do
        if (currentRoom != null && roomBounds.TryGetValue(currentRoom, out Bounds current) && ContainsXZ(current, pos))
        {
            return;
        }

        Transform room = FindRoomAt(pos);

        // In a doorway or gap between rooms, or still in the same room
        if (room == null || room == currentRoom) return;

        currentRoom = room;

        // Only the first time each room is entered
        if (visited.Add(room) && Random.value < CurrentChance)
        {
            SpawnAmbushEnemy(roomBounds[room]);
        }
    }

    void RebuildRooms()
    {
        roomBounds.Clear();

        foreach (RoomBehaviour room in dungeon.GetComponentsInChildren<RoomBehaviour>())
        {
            Renderer[] renderers = room.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) continue;

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            roomBounds[room.transform] = bounds;
        }
    }

    Transform FindRoomAt(Vector3 position)
    {
        foreach (KeyValuePair<Transform, Bounds> room in roomBounds)
        {
            if (ContainsXZ(room.Value, position)) return room.Key;
        }

        return null;
    }

    static bool ContainsXZ(Bounds bounds, Vector3 p)
    {
        return p.x >= bounds.min.x && p.x <= bounds.max.x && p.z >= bounds.min.z && p.z <= bounds.max.z;
    }

    // Spawns an enemy somewhere in the room the player just entered
    void SpawnAmbushEnemy(Bounds room)
    {
        if (ambushEnemies == null || ambushEnemies.Length == 0) return;

        GameObject prefab = ambushEnemies[Random.Range(0, ambushEnemies.Length)];
        if (prefab == null) return;

        Vector3 playerPos = player.transform.position;
        const int attempts = 10;

        for (int n = 0; n < attempts; n++)
        {
            Vector3 pos = new Vector3(
                room.center.x + Random.Range(-room.extents.x, room.extents.x) * 0.7f,
                playerPos.y,
                room.center.z + Random.Range(-room.extents.z, room.extents.z) * 0.7f);

            // Keep the distance from the player, except on the last attempt so something always spawns
            Vector2 flat = new Vector2(pos.x - playerPos.x, pos.z - playerPos.z);
            if (n < attempts - 1 && flat.magnitude < ambushMinDistance) continue;

            if (!NavMesh.SamplePosition(pos, out NavMeshHit hit, 3f, NavMesh.AllAreas)) continue;

            // Parented to the dungeon so it's cleaned up when the next dungeon is generated
            Instantiate(prefab, hit.position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), dungeon.transform);
            return;
        }
    }
}

