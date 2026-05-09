using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Drag your Player object in here!")]
    public Transform target; 
    
    // The camera MUST stay at -10 on the Z axis, otherwise it will be inside 
    // the floor and you won't see anything!
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Camera Feel")]
    [Tooltip("Lower number = tighter follow. Higher number = looser/smoother follow.")]
    public float smoothTime = 0.15f; 
    
    // Unity uses this secretly behind the scenes to calculate the rubber-band math
    private Vector3 velocity = Vector3.zero;

    // We use LateUpdate instead of Update for cameras. 
    // This guarantees the player finishes moving FIRST, and then the camera follows. 
    // If you use normal Update, the camera might jitter!
    void LateUpdate()
    {
        // Safety check: If we forgot to assign the target, just stop right here to prevent errors.
        if (target == null) return;

        // Calculate exactly where the camera wants to go
        Vector3 targetPosition = target.position + offset;

        // Smoothly glide the camera from its current spot to the target spot
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}