using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class DungeonGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;

        public bool obligatory;

        [Tooltip("Rooms of this type can't spawn closer than this many steps (cells) to another room of the same type. 2 = never next to each other, 3 = at least 3 steps apart. 0 = no limit. Ignored for obligatory rooms.")]
        public int minCellsFromSame = 0;

        public int ProbabilityOfSpawning(int x, int y)
        {
            // 0 - cannot spawn 1 - can spawn 2 - HAS to spawn

            // A normal room left at the default (0,0) to (0,0) range can spawn anywhere
            if (!obligatory && minPosition == Vector2Int.zero && maxPosition == Vector2Int.zero)
            {
                return 1;
            }

            if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
            {
                return obligatory ? 2 : 1;
            }

            return 0;
        }
    }

    [Header("Dungeon")]
    public Vector2Int size;
    public int startPos = 0;
    public Vector2 offset;

    [Header("Rooms")]
    public Rule[] rooms;
    public GameObject exitRoom;
    public GameObject spawnRoom;
    public bool customSpawn;
    public Vector2Int spawnCell;

    [Header("Player")]
    public GameObject player;
    public float spawnHeight = 1f;
    public Vector3 landingOffset = Vector3.zero;

    [Header("Exit Door (in the last room)")]
    public GameObject doorPrefab;
    public Vector3 doorOffset = Vector3.zero;
    [UnityEngine.Serialization.FormerlySerializedAs("onFinalExitReached")]
    [UnityEngine.Serialization.FormerlySerializedAs("onFinalBossDefeated")]
    public UnityEngine.Events.UnityEvent onExitReached;

    // A run = one time the dungeon is generated (each load of the scene)
    public static int runCount = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetRunCount()
    {
        runCount = 0;
    }

    private NavMeshSurface navMeshSurface;

    List<Cell> board;

    // Every spawned room, keyed by cell index, so we can find where it really is
    private Dictionary<int, GameObject> spawnedRooms = new Dictionary<int, GameObject>();

    private bool changingRun = false;

    void Start()
    {
        StartRun();
    }

    // Clears the current dungeon and generates a fresh one (the next run)
    public void NextRun()
    {
        // The player has several colliders, so the door can fire more than once. Only react once.
        if (changingRun) return;
        changingRun = true;

        StartCoroutine(NextRunRoutine());
    }

    IEnumerator NextRunRoutine()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        spawnedRooms.Clear();

        // Wait a frame so the old rooms are really gone before building the new ones
        yield return null;

        StartRun();
        changingRun = false;
    }

    void StartRun()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();

        runCount++;

        MazeGenerator();
        CreateExitDoor();

        // Build navigation mesh once, after the dungeon exists
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }

        PlacePlayerAtStart();
    }

    // Moves the player into the start room
    void PlacePlayerAtStart()
    {
        GameObject p = player != null ? player : GameObject.FindGameObjectWithTag("Player");

        if (p == null)
        {
            Debug.LogWarning("DungeonGenerator: no player assigned and nothing tagged 'Player' found.");
            return;
        }

        Vector3 startSpawn = RoomFloorPoint(GetSpawnCell()) + landingOffset + Vector3.up * spawnHeight;
        Debug.Log("DungeonGenerator: placing '" + p.name + "' at " + startSpawn);

        // A CharacterController overrides direct position changes, so switch it off briefly
        CharacterController cc = p.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        p.transform.position = startSpawn;

        Rigidbody rb = p.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = Vector3.zero;

        if (cc != null) cc.enabled = true;
    }

    // World position of a cell's room
    Vector3 CellPosition(int cellIndex)
    {
        int i = cellIndex % size.x;
        int j = cellIndex / size.x;
        return new Vector3(i * offset.x, 0f, -j * offset.y);
    }

    // Center of a room's footprint at floor height. Uses the room's actual visuals,
    // so it still lands in the middle of the room even if the prefab's pivot is in a corner.
    Vector3 RoomFloorPoint(int cellIndex)
    {
        Vector3 fallback = CellPosition(cellIndex);

        if (!spawnedRooms.TryGetValue(cellIndex, out GameObject room) || room == null)
        {
            return fallback;
        }

        Renderer[] renderers = room.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return fallback;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return new Vector3(bounds.center.x, fallback.y, bounds.center.z);
    }

    Transform FindChildByName(Transform root, string childName)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == childName) return t;
        }
        return null;
    }

    // Places the exit door in the last room. Walking into it starts the next run.
    void CreateExitDoor()
    {
        int lastCell = size.x * size.y - 1;
        Vector3 lastRoomPos = RoomFloorPoint(lastCell);

        Vector3 doorPos = lastRoomPos + doorOffset;
        Quaternion doorRot = Quaternion.identity;

        // If the exit room has a child called "ExitDoorSpawn" (or the older "BossDoorSpawn"),
        // the door goes exactly there
        if (spawnedRooms.TryGetValue(lastCell, out GameObject lastRoom) && lastRoom != null)
        {
            Transform marker = FindChildByName(lastRoom.transform, "ExitDoorSpawn");
            if (marker == null) marker = FindChildByName(lastRoom.transform, "BossDoorSpawn");

            if (marker != null)
            {
                doorPos = marker.position;
                doorRot = marker.rotation;
            }
        }

        GameObject door;

        if (doorPrefab != null)
        {
            door = Instantiate(doorPrefab, doorPos, doorRot, transform);
        }
        else
        {
            door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.transform.SetParent(transform);
            door.transform.localScale = new Vector3(2f, 3f, 0.3f);
            door.transform.position = doorPos + Vector3.up * 1.5f;
            door.transform.rotation = doorRot;

            Renderer r = door.GetComponent<Renderer>();
            if (r != null) r.material.color = new Color(0.45f, 0.1f, 0.1f);
        }

        door.name = "Exit Door";

        // Trigger so the player can walk into it
        Collider col = door.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        LayerExit doorExit = door.GetComponent<LayerExit>();
        if (doorExit == null) doorExit = door.AddComponent<LayerExit>();

        // The door starts the next run, forever
        doorExit.teleport = false;
        doorExit.onEntered.AddListener(() => onExitReached.Invoke());
        doorExit.onEntered.AddListener(NextRun);
    }

    // Cell index of the room the player spawns in
    int GetSpawnCell()
    {
        if (!customSpawn)
        {
            return startPos;
        }

        Vector2Int c = spawnCell;

        if (c.x < 0 || c.x >= size.x || c.y < 0 || c.y >= size.y)
        {
            Debug.LogWarning("Spawn cell " + c + " is outside the " + size.x + "x" + size.y + " grid. Using Start Pos.");
            return startPos;
        }

        int index = c.x + c.y * size.x;

        if (index == size.x * size.y - 1)
        {
            Debug.LogWarning("Spawn cell can't be the exit room. Using Start Pos.");
            return startPos;
        }

        return index;
    }

    // Drops any room that would break generation: no prefab assigned, or no RoomBehaviour on it
    void SanitizeRooms()
    {
        if (rooms != null)
        {
            List<Rule> usable = new List<Rule>();

            for (int r = 0; r < rooms.Length; r++)
            {
                if (rooms[r] == null || rooms[r].room == null)
                {
                    Debug.LogError("DungeonGenerator: Rooms element " + r + " has no room prefab assigned, so it is skipped.");
                }
                else if (rooms[r].room.GetComponent<RoomBehaviour>() == null)
                {
                    Debug.LogError("DungeonGenerator: Rooms element " + r + " ('" + rooms[r].room.name + "') has no RoomBehaviour script on its prefab, so it is skipped.");
                }
                else
                {
                    usable.Add(rooms[r]);
                }
            }

            rooms = usable.ToArray();
        }

        if (exitRoom != null && exitRoom.GetComponent<RoomBehaviour>() == null)
        {
            Debug.LogError("DungeonGenerator: Exit Room ('" + exitRoom.name + "') has no RoomBehaviour script, so it is ignored.");
            exitRoom = null;
        }

        if (spawnRoom != null && spawnRoom.GetComponent<RoomBehaviour>() == null)
        {
            Debug.LogError("DungeonGenerator: Spawn Room ('" + spawnRoom.name + "') has no RoomBehaviour script, so it is ignored.");
            spawnRoom = null;
        }
    }

    // True if a room of this type already sits within the rule's minimum distance of (x, y).
    // Distance is counted in steps across the grid.
    static bool TooCloseToSame(Rule rule, List<Vector2Int> placedCells, int x, int y)
    {
        if (rule.minCellsFromSame <= 0) return false;

        foreach (Vector2Int c in placedCells)
        {
            if (Mathf.Abs(c.x - x) + Mathf.Abs(c.y - y) < rule.minCellsFromSame)
            {
                return true;
            }
        }

        return false;
    }

    void GenerateDungeon()
    {
        int lastCellIndex = size.x * size.y - 1;

        SanitizeRooms();

        if (rooms == null || rooms.Length == 0)
        {
            Debug.LogError("No usable rooms. Fill in the 'Rooms' list with prefabs that have a RoomBehaviour script.");
            return;
        }

        // Where each room type has been placed so far, for the minimum-distance rule
        List<Vector2Int>[] placed = new List<Vector2Int>[rooms.Length];
        for (int k = 0; k < placed.Length; k++)
        {
            placed[k] = new List<Vector2Int>();
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[(i + j * size.x)];

                if (currentCell.visited)
                {
                    int randomRoom = -1;
                    List<int> allowedRooms = new List<int>();  // can spawn here and isn't too close to the same type
                    List<int> fittingRooms = new List<int>();  // can spawn here, ignoring distance

                    for (int k = 0; k < rooms.Length; k++)
                    {
                        int p = rooms[k].ProbabilityOfSpawning(i, j);

                        if (p == 2)
                        {
                            randomRoom = k;
                            break;
                        }
                        else if (p == 1)
                        {
                            fittingRooms.Add(k);

                            if (!TooCloseToSame(rooms[k], placed[k], i, j))
                            {
                                allowedRooms.Add(k);
                            }
                        }
                    }

                    if (randomRoom == -1)
                    {
                        // If every room type is too close here, ignore the distance rule rather than fail
                        List<int> pool = allowedRooms.Count > 0 ? allowedRooms : fittingRooms;

                        if (pool.Count > 0)
                        {
                            randomRoom = pool[Random.Range(0, pool.Count)];
                        }
                        else
                        {
                            randomRoom = 0;
                        }
                    }

                    // The final cell uses the dedicated exit room, if one is assigned
                    GameObject roomPrefab = rooms[randomRoom].room;
                    if (i + j * size.x == lastCellIndex && exitRoom != null)
                    {
                        roomPrefab = exitRoom;
                    }

                    // The start cell uses the dedicated spawn room, if one is assigned
                    if (i + j * size.x == GetSpawnCell() && spawnRoom != null)
                    {
                        roomPrefab = spawnRoom;
                    }

                    // Only rooms that really came from the list count for the distance rule
                    if (roomPrefab == rooms[randomRoom].room)
                    {
                        placed[randomRoom].Add(new Vector2Int(i, j));
                    }

                    var newRoom = Instantiate(roomPrefab, new Vector3(i * offset.x, 0f, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();

                    newRoom.UpdateRoom(currentCell.status);
                    newRoom.name += " " + i + "-" + j;
                    spawnedRooms[i + j * size.x] = newRoom.gameObject;
                }
            }
        }
    }

    void MazeGenerator()
    {
        board = new List<Cell>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = GetSpawnCell();

        Stack<int> path = new Stack<int>();

        int k = 0;

        while (k < 1000)
        {
            k++;

            board[currentCell].visited = true;

            if (currentCell == board.Count - 1)
            {
                break;
            }

            //Check the cell's neighbors
            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                if (newCell > currentCell)
                {
                    //down or right
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    //up or left
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }
            }
        }

        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //check up neighbor
        if (cell - size.x >= 0 && !board[(cell - size.x)].visited)
        {
            neighbors.Add((cell - size.x));
        }

        //check down neighbor
        if (cell + size.x < board.Count && !board[(cell + size.x)].visited)
        {
            neighbors.Add((cell + size.x));
        }

        //check right neighbor
        if ((cell + 1) % size.x != 0 && !board[(cell + 1)].visited)
        {
            neighbors.Add((cell + 1));
        }

        //check left neighbor
        if (cell % size.x != 0 && !board[(cell - 1)].visited)
        {
            neighbors.Add((cell - 1));
        }

        return neighbors;
    }
}
