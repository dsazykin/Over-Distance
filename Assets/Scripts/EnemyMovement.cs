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
        rb = GetComponent<Rigidbody2D>();
        
        // Find the player
        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement != null)
        {
            player = playerMovement.transform;
        }

        // Find the pathfinding manager in the scene
        pathfinding = FindFirstObjectByType<Pathfinding>();

        if (pathfinding != null && player != null)
        {
            StartCoroutine(UpdatePathRoutine());
        }
    }

    private System.Collections.IEnumerator UpdatePathRoutine()
    {
        while (true)
        {
            if (!isKnockedBack && player != null)
            {
                currentPath = pathfinding.FindPath(transform.position, player.position);
                targetIndex = 0;
            }
            yield return new WaitForSeconds(pathUpdateInterval);
        }
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
        if (isKnockedBack || player == null || currentPath == null || targetIndex >= currentPath.Count)
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
