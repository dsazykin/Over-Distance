using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Tooltip("How fast this layer moves. 0 is still (sky), 1 moves exactly with the camera.")]
    public float parallaxSpeed; 
    
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    void Start()
    {
        // Find the main camera
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        // Calculate how much the camera moved since last frame
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        
        // Move this background layer slightly based on its unique speed
        transform.position += deltaMovement * parallaxSpeed;
        
        // Save the camera's new position for the next frame
        lastCameraPosition = cameraTransform.position;
    }
}