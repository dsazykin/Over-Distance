using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Events")]
    public UnityEvent<int, int> OnHealthChanged; // (current, max)

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public float flickerDuration = 1f;
    public float flickerInterval = 0.1f;
    private Coroutine flickerCoroutine;
    public Sprite spriteUp;

    void Start()
    {
        currentHealth = maxHealth;
        
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage, float knockbackForce = 0, Vector2 sourcePosition = default)
    {
        // We no longer return early here because enemies handle their own cooldowns.
        // This allows multiple enemies to hit the player in quick succession.

        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"Player took {damage} damage. Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            // Trigger knockback
            if (knockbackForce > 0)
            {
                PlayerMovement movement = GetComponent<PlayerMovement>();
                if (movement != null)
                {
                    Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
                    movement.ApplyKnockback(knockbackForce, direction);
                }
            }

            // Trigger visual feedback
            if (flickerCoroutine != null) StopCoroutine(flickerCoroutine);
            flickerCoroutine = StartCoroutine(HandleFlicker());
        }
    }

    private IEnumerator HandleFlicker()
    {
        // Visual feedback only - no longer grants logic-based invulnerability
        float timer = 0;
        while (timer < flickerDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flickerInterval);
            timer += flickerInterval;
        }
        
        spriteRenderer.enabled = true;
        flickerCoroutine = null;
    }

    private void Die()
    {
        Debug.Log("Player Died!");

        // 1. Stop visual flickering and ensure sprite is visible
        if (flickerCoroutine != null) StopCoroutine(flickerCoroutine);
        spriteRenderer.enabled = true;

        // 2. Disable player scripts
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;
        
        PlayerDash dash = GetComponent<PlayerDash>();
        if (dash != null) dash.enabled = false;

        PlayerAttack attack = GetComponent<PlayerAttack>();
        if (attack != null) attack.enabled = false;

        // 3. Disable the Visuals/Animator
        PlayerVisuals visuals = GetComponent<PlayerVisuals>();
        if (visuals != null) visuals.DisableAnimator();
        
        // 4. Tint the character gray to indicate death
        spriteRenderer.sprite = spriteUp; // In the future this will play the death animation
        spriteRenderer.color = Color.gray;
        
        // In a real game, you'd trigger a Game Over screen or reload the scene
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"Player healed {amount}. Current Health: {currentHealth}");
    }
}
