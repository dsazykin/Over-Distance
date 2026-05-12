using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Knockback Settings")]
    [Range(0f, 1f)]
    public float knockbackResistance = 0f; // 0 = full knockback, 1 = no knockback

    public SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage, float knockbackForce, Vector2 sourcePosition)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHealth}");

        // Handle Knockback
        float finalKnockback = knockbackForce * (1f - knockbackResistance);
        if (finalKnockback > 0)
        {
            EnemyMovement movement = GetComponent<EnemyMovement>();
            if (movement != null)
            {
                Vector2 knockbackDirection = ((Vector2)transform.position - sourcePosition).normalized;
                movement.ApplyKnockback(finalKnockback, knockbackDirection);
            }
        }

        // Visual feedback
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator FlashRed()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        Destroy(gameObject);
    }
}
