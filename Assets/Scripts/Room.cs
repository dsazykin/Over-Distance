using UnityEngine;

public class Room : MonoBehaviour
{
    public enum RoomType { Start, Combat, Treasure, Shop, Boss }

    [Header("Room Info")]
    public Vector2 gridPos;
    public RoomType roomType = RoomType.Combat;
    
    [Header("Exits")]
    public bool hasNorth;
    public bool hasSouth;
    public bool hasEast;
    public bool hasWest;

    [Header("Camera Constraints")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    [Header("Doors")]
    public Door northDoor;
    public Door southDoor;
    public Door eastDoor;
    public Door westDoor;

    [Header("Pathfinding")]
    public PathGrid localGrid;

    public void OnPlayerEnter()
    {
        // 1. Tell the camera to constrain itself to this room
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null)
        {
            cam.SetRoomLimits(minX, maxX, minY, maxY);
        }

        // 2. Reset all parallax layers to prevent jumps
        ParallaxBackground[] parallaxLayers = Object.FindObjectsByType<ParallaxBackground>(FindObjectsSortMode.None);
        foreach (var layer in parallaxLayers)
        {
            layer.ResetParallax();
        }

        // 3. Update enemies ONLY in this room to use this room's grid
        EnemyMovement[] enemiesInRoom = GetComponentsInChildren<EnemyMovement>();
        foreach (var enemy in enemiesInRoom)
        {
            enemy.UpdatePathfindingGrid(localGrid);
        }
    }
}
