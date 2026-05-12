using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage = 10;

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
        // If the collider is a trigger (like the player's weapon hitbox), ignore it!
        // We only want to take damage if the enemy touches the player's body.
        if (collision.isTrigger) return;

        PlayerHealth playerHealth = collision.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
}
