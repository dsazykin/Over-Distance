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
    
    [Header("Visuals")]
    public SpriteRenderer spriteRenderer; 
    
    [Header("Directional Sprites")]
    public Sprite spriteDown;
    public Sprite spriteUp;
    public Sprite spriteSide;

    private Vector2 moveInput;

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        movement = moveInput;

        // Only swap pictures if the player is actually pressing a direction
        if (moveInput != Vector2.zero)
        {
            // Are we moving horizontally (Left/Right) more than vertically (Up/Down)?
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                // Show the Side profile picture
                spriteRenderer.sprite = spriteSide;

                // FLIP LOGIC: Look at which way we are pressing
                if (moveInput.x > 0) 
                {
                    spriteRenderer.flipX = false; // Right
                }
                else if (moveInput.x < 0) 
                {
                    spriteRenderer.flipX = true;  // Left
                }
            }
            else // We are moving vertically more than horizontally
            {
                if (moveInput.y > 0)
                {
                    // Show the Up picture
                    spriteRenderer.sprite = spriteUp;
                }
                else if (moveInput.y < 0)
                {
                    // Show the Down picture
                    spriteRenderer.sprite = spriteDown;
                }
            }
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

        // Change the color to blue when the dash starts!
        spriteRenderer.color = Color.blue; 

        yield return new WaitForSeconds(dashDuration);
        
        isDashing = false; 
        
        // Change it back to white (the default color) when the dash ends!
        spriteRenderer.color = Color.white; 

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    
    
}