using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage = 10;
    public float knockbackForce = 3f;
    public float damageCooldown = 1f; // How often this specific enemy can damage the player
    private float nextDamageTime;
    
    private int hurtboxLayer;

    private void Start()
    {
        hurtboxLayer = LayerMask.NameToLayer("Hurtbox");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Time.time < nextDamageTime) return;

        PlayerHealth playerHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage, knockbackForce, transform.position);
            nextDamageTime = Time.time + damageCooldown;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Time.time < nextDamageTime) return;

        // Only apply damage if the collider is on the Hurtbox layer.
        if (collision.gameObject.layer == hurtboxLayer)
        {
            PlayerHealth playerHealth = collision.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, knockbackForce, transform.position);
                nextDamageTime = Time.time + damageCooldown;
            }
        }
    }
}
