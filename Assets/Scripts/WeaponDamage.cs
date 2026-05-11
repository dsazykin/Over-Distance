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
    }
}
