using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public int damageAmount = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object we hit has an EnemyHealth component
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
        
        if (enemy != null)
        {
            enemy.TakeDamage(damageAmount);
        }
        else
        {
            // Alternative check using tags if preferred
            if (collision.CompareTag("Enemy"))
            {
                // If it has the tag but no component yet, we might want to log it
                Debug.LogWarning("Hit an object tagged 'Enemy' but it has no EnemyHealth component!");
            }
        }
    }
}
