using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage = 10;
    private int hurtboxLayer;

    private void Start()
    {
        hurtboxLayer = LayerMask.NameToLayer("Hurtbox");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Only apply damage if the collider is on the Hurtbox layer.
        // This prevents the enemy from taking damage from (or being blocked by) 
        // the player's weapon hitbox or other triggers.
        if (collision.gameObject.layer == hurtboxLayer)
        {
            PlayerHealth playerHealth = collision.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
