using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public int damageAmount = 20;
    public float knockbackForce = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object we hit has an EnemyHealth component (or its parent does)
        EnemyHealth enemy = collision.GetComponentInParent<EnemyHealth>();
        
        if (enemy != null)
        {
            // We pass the damage, knockback force, and our position to calculate the direction
            enemy.TakeDamage(damageAmount, knockbackForce, transform.position);
        }
    }
}
