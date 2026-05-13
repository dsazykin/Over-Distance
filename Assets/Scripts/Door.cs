using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorDirection { North, South, East, West }
    public DoorDirection direction;
    public Room parentRoom;
    
    // The point where the player will be teleported TO when coming from the opposite direction
    public Transform spawnPoint; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Hurtbox"))
        {
            // Note: Since the player has a complex hierarchy (Body trigger on Hurtbox layer),
            // we check both for safety.
            
            PlayerMovement player = collision.GetComponentInParent<PlayerMovement>();
            if (player != null)
            {
                DungeonGenerator.instance.TransitionToRoom(parentRoom, direction);
            }
        }
    }
}
