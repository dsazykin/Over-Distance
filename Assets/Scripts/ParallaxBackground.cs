using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Tooltip("How fast this layer moves horizontally. 0 is still (sky), 1 moves exactly with the camera.")]
    public float parallaxSpeed; 
    
    private Transform cameraTransform;
    private float lastCameraX;
    private float offsetFromCameraY;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraX = cameraTransform.position.x;
        
        // Save how far we are from the camera vertically at the start
        offsetFromCameraY = transform.position.y - cameraTransform.position.y;
    }

    void LateUpdate()
    {
        // 1. Handle Horizontal Parallax
        float deltaX = cameraTransform.position.x - lastCameraX;
        transform.position += new Vector3(deltaX * parallaxSpeed, 0f, 0f);
        lastCameraX = cameraTransform.position.x;

        // 2. Lock Vertical Position to Camera
        // This ensures the sky stays at the same relative height in every room
        transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetFromCameraY, transform.position.z);
    }

    // Call this when the player moves between rooms to prevent the background from jumping
    public void ResetParallax()
    {
        lastCameraX = cameraTransform.position.x;
    }
}