using UnityEngine;
using UnityEngine.InputSystem; // We MUST add this to use the new system!

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    private Vector2 movement;

    // Because we set the Behavior to "Send Messages", Unity will automatically
    // search for and trigger this "OnMove" function whenever you press WASD,
    // the arrow keys, or use a controller joystick.
    void OnMove(InputValue value)
    {
        // Read the 2D input (X and Y) and store it in our movement variable
        movement = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        // Move the physical body to its current position + (our input * speed * time)
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}