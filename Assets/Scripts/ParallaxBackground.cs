using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Tooltip("How fast this layer moves horizontally. 0 is still (sky), 1 moves exactly with the camera.")]
    public float parallaxSpeedX; 
    [Tooltip("How fast this layer moves vertically. Only works if Lock Vertical is unchecked.")]
    public float parallaxSpeedY;
    
    [Tooltip("If checked, the layer stays at a fixed Y offset from the camera (best for sky/far background).")]
    public bool lockVertical = true;
    
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float offsetFromCameraY;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        
        // Save how far we are from the camera vertically at the start
        offsetFromCameraY = transform.position.y - cameraTransform.position.y;
    }

    void LateUpdate()
    {
        Vector3 deltaCamera = cameraTransform.position - lastCameraPosition;

        // 1. Handle Horizontal Parallax
        transform.position += new Vector3(deltaCamera.x * parallaxSpeedX, 0f, 0f);

        // 2. Handle Vertical Movement
        if (lockVertical)
        {
            // Lock Vertical Position to Camera (Sky Mode)
            transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetFromCameraY, transform.position.z);
        }
        else
        {
            // Apply Vertical Parallax (Foreground/Depth Mode)
            transform.position += new Vector3(0f, deltaCamera.y * parallaxSpeedY, 0f);
        }

        lastCameraPosition = cameraTransform.position;
    }

    // Call this when the player moves between rooms to prevent the background from jumping
    public void ResetParallax()
    {
        if (cameraTransform == null) 
        {
            cameraTransform = Camera.main.transform;
        }
        lastCameraPosition = cameraTransform.position;
    }
}