using UnityEngine;
using UnityEngine.InputSystem;

// Handles player movement for the UFO.
// The player uses WASD to move around the farm.
// Movement can also be locked temporarily during events
// such as the abduction minigame or opening countdown.
public class UfoMovement : MonoBehaviour
{
    // Movement speed measured in world units per second.
    public float speed = 5f;

    // Determines whether the player is currently
    // allowed to move the UFO.
    private bool canMove = true;

    void Update()
    {
        // Ignore movement input while movement is locked.
        if (!canMove)
            return;

        // Start with no movement input each frame.
        Vector2 input = Vector2.zero;

        // Only check keyboard input if a keyboard is available.
        if (Keyboard.current != null)
        {
            // Move left.
            if (Keyboard.current.aKey.isPressed)
                input.x -= 1;

            // Move right.
            if (Keyboard.current.dKey.isPressed)
                input.x += 1;

            // Move down.
            if (Keyboard.current.sKey.isPressed)
                input.y -= 1;

            // Move up.
            if (Keyboard.current.wKey.isPressed)
                input.y += 1;
        }

        // Convert the 2D input into a Vector3
        // so it can be applied to the UFO's Transform.
        Vector3 direction =
            new Vector3(input.x, input.y, 0f);

        // Normalize the direction so diagonal movement
        // is not faster than horizontal or vertical movement.
        transform.position +=
            direction.normalized *
            speed *
            Time.deltaTime;
    }

    // Prevents the UFO from moving.
    // Used during gameplay events where movement should be disabled.
    public void LockMovement()
    {
        canMove = false;
    }

    // Allows the UFO to move again.
    public void UnlockMovement()
    {
        canMove = true;
    }
}