using UnityEngine;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Transform player;
    private Rigidbody2D rb;

    [Header("Pathfinding Settings")]
    public float pathUpdateInterval = 0.2f;
    public float waypointThreshold = 0.2f;
    private Pathfinding pathfinding;
    private List<Vector2> currentPath;
    private int targetIndex;

    [Header("Knockback Settings")]
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;

    void Start()
    {
        InitializeComponents();
        StartCoroutine(UpdatePathRoutine());
    }

    private void InitializeComponents()
    {
        if (rb != null) return; // Already initialized

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
        
        // Find the player
        PlayerMovement playerMovement = Object.FindFirstObjectByType<PlayerMovement>();
        if (playerMovement != null)
        {
            player = playerMovement.transform;
        }

        // Add a Pathfinding component if this enemy doesn't have one
        pathfinding = GetComponent<Pathfinding>();
        if (pathfinding == null)
        {
            pathfinding = gameObject.AddComponent<Pathfinding>();
        }

        // Ensure the main collider is NOT a trigger for wall collisions
        Collider2D mainCollider = GetComponent<Collider2D>();
        if (mainCollider != null)
        {
            mainCollider.isTrigger = false;
        }

        // Automatically find our room's grid if we don't have one
        Room myRoom = GetComponentInParent<Room>();
        if (myRoom != null && myRoom.localGrid != null && pathfinding != null)
        {
            pathfinding.SetGrid(myRoom.localGrid);
        }
    }

    public void UpdatePathfindingGrid(PathGrid newGrid)
    {
        InitializeComponents();
        if (pathfinding != null)
        {
            pathfinding.SetGrid(newGrid);
        }
    }

    private System.Collections.IEnumerator UpdatePathRoutine()
    {
        while (true)
        {
            // 1. Wait until we actually have a grid and a player target
            if (pathfinding != null && pathfinding.HasGrid && player != null)
            {
                // 2. Only pathfind if we are in the SAME room as the player
                if (IsPlayerInRoom())
                {
                    currentPath = pathfinding.FindPath(transform.position, player.position);
                    targetIndex = 0;
                }
                else
                {
                    currentPath = null;
                }
            }
            yield return new WaitForSeconds(pathUpdateInterval);
        }
    }

    private bool IsPlayerInRoom()
    {
        Room myRoom = GetComponentInParent<Room>();
        if (myRoom == null) return false;

        // Simple check: is the player within the room's camera limits?
        // (This is a good proxy for "is the player in this room")
        float x = player.position.x;
        float y = player.position.y;
        
        bool withinX = x > myRoom.transform.position.x + myRoom.minX - 5 && x < myRoom.transform.position.x + myRoom.maxX + 5;
        bool withinY = y > myRoom.transform.position.y + myRoom.minY - 5 && y < myRoom.transform.position.y + myRoom.maxY + 5;
        
        return withinX && withinY;
    }

    public void ApplyKnockback(float force, Vector2 direction)
    {
        if (!isKnockedBack)
        {
            StartCoroutine(KnockbackRoutine(force, direction));
        }
    }

    private System.Collections.IEnumerator KnockbackRoutine(float force, Vector2 direction)
    {
        isKnockedBack = true;
        
        // We use velocity for the knockback burst
        rb.linearVelocity = direction * force;
        
        yield return new WaitForSeconds(knockbackDuration);
        
        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }

    void FixedUpdate()
    {
        if (isKnockedBack || player == null)
        {
            return;
        }

        if (currentPath == null || targetIndex >= currentPath.Count)
        {
            return;
        }

        // Follow the current path
        Vector2 targetWaypoint = currentPath[targetIndex];
        Vector2 direction = (targetWaypoint - (Vector2)transform.position).normalized;
        
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        // Check if we reached the current waypoint
        if (Vector2.Distance(transform.position, targetWaypoint) < waypointThreshold)
        {
            targetIndex++;
        }
    }
}
