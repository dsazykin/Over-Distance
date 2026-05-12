using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    public Animator animator;
    
    [Header("Directional Sprites")]
    public Sprite spriteDown;
    public Sprite spriteUp;
    public Sprite spriteSide;

    [Header("Dash Sprites")]
    public Sprite dashSpriteDown;
    public Sprite dashSpriteUp;
    public Sprite dashSpriteSide;
    
    [Header("Combat Settings")]
    public GameObject weaponHitbox;
    public float attackDuration = 0.15f; // How long the hitbox stays active
    private bool isAttacking = false;

    private Vector2 moveInput;
    
    // Animator Hashes
    private static readonly int WalkFrontHash = Animator.StringToHash("Walk_Front");
    private static readonly int WalkSideHash = Animator.StringToHash("Walk_Side");
    private static readonly int WalkBackHash = Animator.StringToHash("Walk_Back");
    
    private static readonly int AttackFrontHash = Animator.StringToHash("Attack_Front");
    private static readonly int AttackSideHash = Animator.StringToHash("Attack_Side");
    // private static readonly int AttackBackHash = Animator.StringToHash("Attack_Back");
    
    void Start()
    {
        // Automatically find the Animator that Unity added to your player
        animator = GetComponent<Animator>();
        
        animator.enabled = false;

        // Force the starting picture to be the "facing down" sprite
        spriteRenderer.sprite = spriteDown;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        movement = moveInput;

        // Only update animations if we aren't currently dashing
        if (!isDashing)
        {
            UpdateAnimationState();
        }
    }

    private void UpdateAnimationState()
    {
        if (moveInput != Vector2.zero)
        {
            lastMovement = moveInput;
            animator.enabled = true;

            // Horizontal movement priority
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                animator.Play(WalkSideHash);

                if (moveInput.x > 0) spriteRenderer.flipX = false;
                else if (moveInput.x < 0) spriteRenderer.flipX = true;
            }
            else // Vertical movement
            {
                if (moveInput.y > 0)
                {
                    animator.Play(WalkBackHash);
                }
                else if (moveInput.y < 0)
                {
                    animator.Play(WalkFrontHash);
                }
            }
        }
        else
        {
            animator.enabled = false;

            if (Mathf.Abs(lastMovement.x) > Mathf.Abs(lastMovement.y))
            {
                spriteRenderer.sprite = spriteSide;
                if (lastMovement.x < 0) spriteRenderer.flipX = true;
                else spriteRenderer.flipX = false;
            }
            else
            {
                if (lastMovement.y > 0) spriteRenderer.sprite = spriteUp;
                else spriteRenderer.sprite = spriteDown;
            }
        }
    }

    // Because we named the action "Dash" in the input settings, 
    // Unity automatically looks for a function named "OnDash"
    void OnDash(InputValue value) {
        // Only dash if the button was pressed, we aren't already dashing, and cooldown is done
        if (value.isPressed && canDash && !isDashing){
            StartCoroutine(PerformDash());
        }
    }

    void FixedUpdate() {
        // If we are currently dashing, ignore normal movement and apply dash velocity
        if (isDashing) {
            rb.MovePosition(rb.position + lastMovement * dashSpeed * Time.fixedDeltaTime);
            return; // This stops the rest of the FixedUpdate code from running
        }

        // Stop movement while attacking
        if (isAttacking) {
            return;
        }

        // Normal walking movement
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    // A Coroutine acts like a mini-timeline that we can pause
    private IEnumerator PerformDash() {
        canDash = false;
        isDashing = true;

        // 1. Disable animator so it doesn't override our dash sprite
        animator.enabled = false;

        // 2. Set the correct dash sprite based on direction
        if (Mathf.Abs(lastMovement.x) > Mathf.Abs(lastMovement.y)) {
            spriteRenderer.sprite = dashSpriteSide;
            spriteRenderer.flipX = lastMovement.x < 0;
        } else {
            spriteRenderer.sprite = lastMovement.y > 0 ? dashSpriteUp : dashSpriteDown;
        }

        yield return new WaitForSeconds(dashDuration);
        
        isDashing = false; 

        // 3. Return to normal animation/sprite state
        UpdateAnimationState();

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    
    // Unity automatically calls this because you named the input action "Attack"
    void OnAttack(InputValue value) {
        if (value.isPressed && !isAttacking && !isDashing){
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack() {
        isAttacking = true;
        // animator.enabled = true;

        // 1. Move the hitbox to face the correct direction!
        // We use lastMovement.normalized to get a clean direction (Up, Down, Left, or Right)
        // and multiply it by 0.7f to push it 0.7 units out from the player's center.
        if (lastMovement.x == 0) {
            if (lastMovement.y > 0) {
                weaponHitbox.transform.localRotation = Quaternion.Euler(0f, 0f, 90f); // Rotate the hitbox to be vertical
                // animator.Play(AttackBackHash);
            }
            else if (lastMovement.y < 0) {
                weaponHitbox.transform.localRotation = Quaternion.Euler(0f, 0f, -90f); // Rotate the hitbox to be vertical
                // animator.Play(AttackFrontHash);
            }
            
            weaponHitbox.transform.localPosition = new Vector3(0f, lastMovement.y, 0f).normalized * 0.7f;
            
        } else if ( lastMovement.y == 0){
            if (lastMovement.x > 0) {
                weaponHitbox.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); // Rotate the hitbox to be vertical
                // animator.Play(AttackSideHash);
                // spriteRenderer.flipX = false;
            }
            else if (lastMovement.x < 0) {
                weaponHitbox.transform.localRotation = Quaternion.Euler(0f, 0f, 180f); // Rotate the hitbox to be vertical
                // animator.Play(AttackSideHash);
                // spriteRenderer.flipX = true;
            }
            
            weaponHitbox.transform.localPosition = new Vector3(lastMovement.x, 0f, 0f).normalized * 0.7f;
            
        } else {
            if (Mathf.Abs(lastMovement.x) > Mathf.Abs(lastMovement.y)) {
                if (lastMovement.x > 0) {
                    weaponHitbox.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    // animator.Play(AttackSideHash);
                    // spriteRenderer.flipX = false;
                }
                else if (lastMovement.x < 0) {
                    weaponHitbox.transform.localRotation = Quaternion.Euler(0f, 0f, -180f);
                    // animator.Play(AttackSideHash);
                    // spriteRenderer.flipX = true;
                }
                weaponHitbox.transform.localPosition = new Vector3(lastMovement.x, 0f, 0f).normalized * 0.7f;
            }
            else {
                if (lastMovement.y > 0) {
                    weaponHitbox.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                    // animator.Play(AttackBackHash);
                }
                else if (lastMovement.y < 0) {
                    weaponHitbox.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
                    // animator.Play(AttackFrontHash);
                }
                weaponHitbox.transform.localPosition = new Vector3(0f, lastMovement.y, 0f).normalized * 0.7f;
            }
        }

        // 2. Turn the hitbox ON
        weaponHitbox.SetActive(true);

        // 3. Wait for the duration of the swing
        yield return new WaitForSeconds(attackDuration);

        // 4. Turn the hitbox OFF
        weaponHitbox.SetActive(false);

        isAttacking = false;
        
        // Return to normal animation/sprite state
        // UpdateAnimationState();
    }
    
}