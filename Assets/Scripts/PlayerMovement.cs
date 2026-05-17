using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    
    private Vector2 moveInput;
    private Vector2 movement;
    private Vector2 lastDirection = Vector2.down;
    public Vector2 LastDirection => lastDirection;

    [Header("Knockback Settings")]
    public float knockbackDuration = 0.15f;
    private bool isKnockedBack;
    public bool IsKnockedBack => isKnockedBack;

    private PlayerVisuals visuals;
    private PlayerDash dash;
    private PlayerAttack attack;

    public bool IsDashing => dash != null && dash.IsDashing;
    public bool IsAttacking => attack != null && attack.IsAttacking;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        visuals = GetComponent<PlayerVisuals>();
        dash = GetComponent<PlayerDash>();
        attack = GetComponent<PlayerAttack>();
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        movement = moveInput;

        if (moveInput != Vector2.zero)
        {
            lastDirection = moveInput;
        }

        if (!IsDashing && !IsAttacking && !IsKnockedBack)
        {
            RefreshVisuals();
        }
    }

    private void FixedUpdate()
    {
        if (IsKnockedBack || IsDashing || IsAttacking)
        {
            return;
        }

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public void RefreshVisuals()
    {
        visuals.UpdateVisuals(moveInput, lastDirection, moveInput != Vector2.zero);
    }

    public void ApplyKnockback(float force, Vector2 direction)
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(KnockbackRoutine(force, direction));
        }
    }

    private IEnumerator KnockbackRoutine(float force, Vector2 direction)
    {
        isKnockedBack = true;
        
        // Interrupt other actions
        if (dash != null) dash.InterruptDash();
        if (attack != null) attack.InterruptAttack();

        rb.linearVelocity = direction * force;
        yield return new WaitForSeconds(knockbackDuration);
        rb.linearVelocity = Vector2.zero;
        
        isKnockedBack = false;
        RefreshVisuals();
    }
}
