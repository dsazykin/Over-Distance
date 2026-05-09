using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // We must add this to use Coroutines!

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    private Vector2 movement;
    
    // We store the last direction moved. If the player presses dash while 
    // standing still, they will dash in the direction they are facing!
    private Vector2 lastMovement = Vector2.right; 

    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private bool isDashing;
    private bool canDash = true;

    void OnMove(InputValue value)
    {
        // Read the input
        movement = value.Get<Vector2>();

        // If the player is currently pressing a direction, save it
        if (movement != Vector2.zero)
        {
            lastMovement = movement.normalized;
        }
    }

    // Because we named the action "Dash" in the input settings, 
    // Unity automatically looks for a function named "OnDash"
    void OnDash(InputValue value)
    {
        // Only dash if the button was pressed, we aren't already dashing, and cooldown is done
        if (value.isPressed && canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    void FixedUpdate()
    {
        // If we are currently dashing, ignore normal movement and apply dash velocity
        if (isDashing)
        {
            rb.MovePosition(rb.position + lastMovement * dashSpeed * Time.fixedDeltaTime);
            return; // This stops the rest of the FixedUpdate code from running
        }

        // Normal walking movement
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    // A Coroutine acts like a mini-timeline that we can pause
    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        // Wait for a fraction of a second while the player zooms forward
        yield return new WaitForSeconds(dashDuration);
        isDashing = false; // Stop dashing

        // Wait for the cooldown timer to finish before they can dash again
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}