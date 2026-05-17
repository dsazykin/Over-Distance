using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Combat Settings")]
    public GameObject weaponHitbox;
    public float attackDuration = 0.15f;
    
    private bool isAttacking;
    public bool IsAttacking => isAttacking;

    private PlayerMovement movement;
    private PlayerVisuals visuals;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        visuals = GetComponent<PlayerVisuals>();
        weaponHitbox.SetActive(false);
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed && !isAttacking && !movement.IsDashing && !movement.IsKnockedBack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        visuals.DisableAnimator();

        Vector2 lastMovement = movement.LastDirection;

        // Position and rotate hitbox
        if (Mathf.Abs(lastMovement.x) > Mathf.Abs(lastMovement.y))
        {
            weaponHitbox.transform.localRotation = Quaternion.Euler(0f, 0f, lastMovement.x > 0 ? 0f : 180f);
            weaponHitbox.transform.localPosition = new Vector3(lastMovement.x > 0 ? 0.7f : -0.7f, 0f, 0f);
        }
        else
        {
            weaponHitbox.transform.localRotation = Quaternion.Euler(0f, 0f, lastMovement.y > 0 ? 90f : -90f);
            weaponHitbox.transform.localPosition = new Vector3(0f, lastMovement.y > 0 ? 0.7f : -0.7f, 0f);
        }

        weaponHitbox.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        weaponHitbox.SetActive(false);

        isAttacking = false;
        movement.RefreshVisuals();
    }

    public void InterruptAttack()
    {
        StopAllCoroutines();
        isAttacking = false;
        weaponHitbox.SetActive(false);
    }
}
