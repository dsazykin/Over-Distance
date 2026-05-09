using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; 
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Camera Feel")]
    public float smoothTime = 0.15f; 
    private Vector3 velocity = Vector3.zero;

    [Header("Camera Limits (Up & Down)")]
    public bool useLimits = true;
    public float minY = -5f; // The lowest the camera can go
    public float maxY = 5f;  // The highest the camera can go

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate exactly where the camera wants to go
        Vector3 targetPosition = target.position + offset;

        // If limits are turned on, restrict the Y (Up/Down) movement
        if (useLimits)
        {
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        // Smoothly glide the camera from its current spot to the target spot
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}