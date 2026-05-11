using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("I-Frames")]
    public float iFrameDuration = 1f;
    private bool isInvulnerable = false;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public float flickerInterval = 0.1f;

    void Start()
    {
        currentHealth = maxHealth;
        
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            StartCoroutine(HandleIFrames());
        }
    }

    private IEnumerator HandleIFrames()
    {
        isInvulnerable = true;
        
        // Visual feedback: Flickering
        float timer = 0;
        while (timer < iFrameDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flickerInterval);
            timer += flickerInterval;
        }
        
        spriteRenderer.enabled = true;
        isInvulnerable = false;
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        // For now, let's just disable the player movement
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }
        
        // Maybe change color to gray or something to indicate death
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
        Debug.Log($"Player healed {amount}. Current Health: {currentHealth}");
    }
}
