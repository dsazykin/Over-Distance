using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public UnityEvent<float> OnDashCooldownChanged;

    private bool isDashing;
    public bool IsDashing => isDashing;
    private bool canDash = true;

    private PlayerMovement movement;
    private PlayerVisuals visuals;
    private Rigidbody2D rb;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        visuals = GetComponent<PlayerVisuals>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed && canDash && !isDashing && !movement.IsKnockedBack)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        visuals.SetDashVisuals(movement.LastDirection);

        float timer = 0;
        while (timer < dashDuration)
        {
            rb.MovePosition(rb.position + movement.LastDirection * dashSpeed * Time.fixedDeltaTime);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
        movement.RefreshVisuals();

        // Cooldown
        float cooldownTimer = 0;
        while (cooldownTimer < dashCooldown)
        {
            cooldownTimer += Time.deltaTime;
            OnDashCooldownChanged?.Invoke(cooldownTimer / dashCooldown);
            yield return null;
        }

        OnDashCooldownChanged?.Invoke(1f);
        canDash = true;
    }

    public void InterruptDash()
    {
        StopAllCoroutines();
        isDashing = false;
        // We don't necessarily reset canDash here unless we want to penalize
    }
}
