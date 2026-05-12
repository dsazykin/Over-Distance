using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Transform player;
    private Rigidbody2D rb;

    [Header("Knockback Settings")]
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Find the player by component instead of tag
        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement != null)
        {
            player = playerMovement.transform;
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
        rb.velocity = direction * force;
        
        yield return new WaitForSeconds(knockbackDuration);
        
        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }

    void FixedUpdate()
    {
        if (player != null && !isKnockedBack)
        {
            // Simple follow logic
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
