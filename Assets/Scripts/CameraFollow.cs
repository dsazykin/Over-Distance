using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; 
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Camera Feel")]
    public float smoothTime = 0.15f; 
    private Vector3 velocity = Vector3.zero;

    [Header("Camera Limits")]
    public bool useLimits = true;
    public float minX, maxX;
    public float minY, maxY;

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate where the camera wants to go
        Vector3 targetPosition = target.position + offset;

        // If limits are turned on, restrict the movement
        if (useLimits)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        // Smoothly glide the camera
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    public void SetRoomLimits(float xMin, float xMax, float yMin, float yMax)
    {
        minX = xMin;
        maxX = xMax;
        minY = yMin;
        maxY = yMax;
        useLimits = true;
    }
}