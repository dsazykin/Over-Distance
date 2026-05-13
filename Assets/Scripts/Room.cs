using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Room Info")]
    public Vector2 gridPos;
    
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

    public void OnPlayerEnter()
    {
        // Tell the camera to constrain itself to this room
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null)
        {
            cam.SetRoomLimits(minX, maxX, minY, maxY);
        }

        // Reset all parallax layers to prevent jumps
        ParallaxBackground[] parallaxLayers = Object.FindObjectsByType<ParallaxBackground>(FindObjectsSortMode.None);
        foreach (var layer in parallaxLayers)
        {
            layer.ResetParallax();
        }
    }
}
