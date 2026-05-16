using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator instance;

    [Header("Generation Settings")]
    public int maxRooms = 10;
    public Vector2 roomSpacing = new Vector2(30f, 20f);
    
    [Header("Room Pool")]
    public RoomPreset startRoomPreset;
    public List<RoomPreset> roomPresets;
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
        // 1. Spawn the Start Room
        if (startRoomPreset == null)
        {
            Debug.LogError("No Start Room Preset assigned to DungeonGenerator!");
            return;
        }

        Room startRoom = SpawnRoom(Vector2.zero, startRoomPreset.roomPrefab);
        startRoom.OnPlayerEnter(); // Set initial camera limits

        Queue<Vector2> roomQueue = new Queue<Vector2>();
        roomQueue.Enqueue(Vector2.zero);

        int roomsSpawned = 1;

        // 2. Branch out from the start room
        while (roomQueue.Count > 0 && roomsSpawned < maxRooms)
        {
            Vector2 currentPos = roomQueue.Dequeue();
            Room currentRoom = dungeonGrid[currentPos];

            // Try to spawn neighbors in each available direction
            if (currentRoom.hasNorth) TrySpawnNeighbor(currentPos + Vector2.up, Door.DoorDirection.North, ref roomsSpawned, roomQueue);
            if (currentRoom.hasSouth) TrySpawnNeighbor(currentPos + Vector2.down, Door.DoorDirection.South, ref roomsSpawned, roomQueue);
            if (currentRoom.hasEast)  TrySpawnNeighbor(currentPos + Vector2.right, Door.DoorDirection.East, ref roomsSpawned, roomQueue);
            if (currentRoom.hasWest)  TrySpawnNeighbor(currentPos + Vector2.left, Door.DoorDirection.West, ref roomsSpawned, roomQueue);
        }

        // 3. Seal any doors that don't lead anywhere
        SealUnconnectedDoors();
    }

    void TrySpawnNeighbor(Vector2 targetPos, Door.DoorDirection fromDirection, ref int roomsSpawned, Queue<Vector2> queue)
    {
        if (dungeonGrid.ContainsKey(targetPos) || roomsSpawned >= maxRooms) return;

        // Find all presets that have the REQUIRED connecting door
        List<RoomPreset> validPresets = new List<RoomPreset>();
        foreach (var preset in roomPresets)
        {
            Room roomScript = preset.roomPrefab.GetComponent<Room>();
            bool hasRequiredDoor = false;
            
            // If we are moving North, the new room must have a South door to connect back
            switch (fromDirection)
            {
                case Door.DoorDirection.North: hasRequiredDoor = roomScript.hasSouth; break;
                case Door.DoorDirection.South: hasRequiredDoor = roomScript.hasNorth; break;
                case Door.DoorDirection.East:  hasRequiredDoor = roomScript.hasWest; break;
                case Door.DoorDirection.West:  hasRequiredDoor = roomScript.hasEast; break;
            }

            if (hasRequiredDoor) validPresets.Add(preset);
        }

        if (validPresets.Count > 0)
        {
            // Pick based on weights
            RoomPreset selectedPreset = PickWeightedPreset(validPresets);
            SpawnRoom(targetPos, selectedPreset.roomPrefab);
            roomsSpawned++;
            queue.Enqueue(targetPos);
        }
    }

    RoomPreset PickWeightedPreset(List<RoomPreset> presets)
    {
        int totalWeight = 0;
        foreach (var p in presets) totalWeight += p.weight;
        
        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;
        
        foreach (var p in presets)
        {
            currentWeight += p.weight;
            if (randomValue < currentWeight) return p;
        }
        
        return presets[0];
    }

    void SealUnconnectedDoors()
    {
        foreach (var entry in dungeonGrid)
        {
            Vector2 pos = entry.Key;
            Room room = entry.Value;

            if (room.hasNorth && !dungeonGrid.ContainsKey(pos + Vector2.up)) SealDoor(room.northDoor);
            if (room.hasSouth && !dungeonGrid.ContainsKey(pos + Vector2.down)) SealDoor(room.southDoor);
            if (room.hasEast && !dungeonGrid.ContainsKey(pos + Vector2.right)) SealDoor(room.eastDoor);
            if (room.hasWest && !dungeonGrid.ContainsKey(pos + Vector2.left)) SealDoor(room.westDoor);
        }
    }

    void SealDoor(Door door)
    {
        if (door != null && wallBlockPrefab != null)
        {
            Instantiate(wallBlockPrefab, door.transform.position, Quaternion.identity, door.transform.parent);
            door.gameObject.SetActive(false);
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
