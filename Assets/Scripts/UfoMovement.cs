using UnityEngine;
using UnityEngine.InputSystem;

public class UfoMovement : MonoBehaviour
{
    // Movement speed in world units per second.
    public float speed = 5f;

    void Update()
    {
        // Start with no movement when no movement key is pressed.
        Vector2 input = Vector2.zero;

        // Read movement keys through Unity's Input System when a keyboard is available.
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
                input.x -= 1;

            if (Keyboard.current.dKey.isPressed)
                input.x += 1;

            if (Keyboard.current.sKey.isPressed)
                input.y -= 1;

            if (Keyboard.current.wKey.isPressed)
                input.y += 1;
        }

        // Convert the 2D keyboard input into a 3D direction on the XY plane.
        Vector3 direction = new Vector3(input.x, input.y, 0f);

        // Normalize diagonal movement and scale it by speed and frame time.
        transform.position += direction.normalized * speed * Time.deltaTime;
    }
}