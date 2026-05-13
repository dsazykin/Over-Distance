using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator instance;

    [Header("Generation Settings")]
    public int maxRooms = 10;
    public Vector2 roomSpacing = new Vector2(30f, 20f);
    
    [Header("Room Pool")]
    public GameObject startRoomPrefab;
    public List<GameObject> roomPrefabs;
    public GameObject wallBlockPrefab; // To seal unused doors

    private Dictionary<Vector2, Room> dungeonGrid = new Dictionary<Vector2, Room>();
    private PlayerMovement player;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        player = Object.FindFirstObjectByType<PlayerMovement>();
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        // For now, let's just spawn the start room at 0,0
        // Phase 2 will involve the full branching algorithm
        SpawnRoom(Vector2.zero, startRoomPrefab);
        
        // Finalize A* grid after rooms are placed
        PathGrid pathGrid = Object.FindFirstObjectByType<PathGrid>();
        if (pathGrid != null)
        {
            // We'll need to make sure PathGrid is updated to scan the whole generated area
            // pathGrid.GenerateGrid(); 
        }
    }

    Room SpawnRoom(Vector2 gridPos, GameObject prefab)
    {
        Vector3 worldPos = new Vector3(gridPos.x * roomSpacing.x, gridPos.y * roomSpacing.y, 0);
        GameObject roomObj = Instantiate(prefab, worldPos, Quaternion.identity);
        Room room = roomObj.GetComponent<Room>();
        room.gridPos = gridPos;
        dungeonGrid[gridPos] = room;
        return room;
    }

    public void TransitionToRoom(Room currentRoom, Door.DoorDirection exitDirection)
    {
        Vector2 targetGridPos = currentRoom.gridPos;

        // Calculate target grid coordinate
        switch (exitDirection)
        {
            case Door.DoorDirection.North: targetGridPos.y += 1; break;
            case Door.DoorDirection.South: targetGridPos.y -= 1; break;
            case Door.DoorDirection.East:  targetGridPos.x += 1; break;
            case Door.DoorDirection.West:  targetGridPos.x -= 1; break;
        }

        if (dungeonGrid.ContainsKey(targetGridPos))
        {
            Room nextRoom = dungeonGrid[targetGridPos];
            
            // Find the corresponding entry door in the next room
            Door entryDoor = null;
            switch (exitDirection)
            {
                case Door.DoorDirection.North: entryDoor = nextRoom.southDoor; break;
                case Door.DoorDirection.South: entryDoor = nextRoom.northDoor; break;
                case Door.DoorDirection.East:  entryDoor = nextRoom.westDoor; break;
                case Door.DoorDirection.West:  entryDoor = nextRoom.eastDoor; break;
            }

            if (entryDoor != null && entryDoor.spawnPoint != null)
            {
                // Teleport player
                player.transform.position = entryDoor.spawnPoint.position;
                
                // Update Camera & Room state
                nextRoom.OnPlayerEnter();
            }
        }
    }
}
