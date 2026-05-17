using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [Header("Visual Components")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("Directional Sprites (Idle)")]
    public Sprite spriteDown;
    public Sprite spriteUp;
    public Sprite spriteSide;

    [Header("Dash Sprites")]
    public Sprite dashSpriteDown;
    public Sprite dashSpriteUp;
    public Sprite dashSpriteSide;

    // Animator Hashes
    private static readonly int WalkFrontHash = Animator.StringToHash("Walk_Front");
    private static readonly int WalkSideHash = Animator.StringToHash("Walk_Side");
    private static readonly int WalkBackHash = Animator.StringToHash("Walk_Back");

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        
        animator.enabled = false;
        spriteRenderer.sprite = spriteDown;
    }

    public void UpdateVisuals(Vector2 moveInput, Vector2 lastDirection, bool isMoving)
    {
        if (isMoving)
        {
            animator.enabled = true;

            // Horizontal movement priority
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                animator.Play(WalkSideHash);
                spriteRenderer.flipX = moveInput.x < 0;
            }
            else // Vertical movement
            {
                if (moveInput.y > 0) animator.Play(WalkBackHash);
                else animator.Play(WalkFrontHash);
            }
        }
        else
        {
            animator.enabled = false;

            if (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
            {
                spriteRenderer.sprite = spriteSide;
                spriteRenderer.flipX = lastDirection.x < 0;
            }
            else
            {
                spriteRenderer.sprite = lastDirection.y > 0 ? spriteUp : spriteDown;
            }
        }
    }

    public void SetDashVisuals(Vector2 direction)
    {
        animator.enabled = false;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            spriteRenderer.sprite = dashSpriteSide;
            spriteRenderer.flipX = direction.x < 0;
        }
        else
        {
            spriteRenderer.sprite = direction.y > 0 ? dashSpriteUp : dashSpriteDown;
        }
    }

    public void DisableAnimator() => animator.enabled = false;
    public void SetColor(Color color) => spriteRenderer.color = color;
    public void SetSprite(Sprite sprite) => spriteRenderer.sprite = sprite;
}
