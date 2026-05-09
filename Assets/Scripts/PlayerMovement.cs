using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // The "public" keyword makes this show up in the Unity Editor, 
    // so you can change the speed later without opening Rider!
    public float moveSpeed = 5f;

    // This is a reference to the physics component we will attach to your player.
    public Rigidbody2D rb;

    // This variable stores your horizontal and vertical input (X and Y)
    private Vector2 movement;

    // Update is called once every single frame. 
    // It is the best place to check for player button presses.
    void Update()
    {
        // GetAxisRaw checks for WASD or Arrow Keys. 
        // It returns -1 (left/down), 1 (right/up), or 0 (no button pressed).
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // We "normalize" the movement so that walking diagonally isn't 
        // twice as fast as walking in a straight line.
        movement = movement.normalized;
    }

    // FixedUpdate runs at a steady, fixed rate (like 50 times a second). 
    // You MUST put all physics/movement math in here, not in Update.
    void FixedUpdate()
    {
        // Move the physical body to its current position + (our input * speed * time)
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}