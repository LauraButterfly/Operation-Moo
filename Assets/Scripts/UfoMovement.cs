using UnityEngine;
using UnityEngine.InputSystem;

public class UfoMovement : MonoBehaviour
{
    // Movement speed in world units per second.
    public float speed = 5f;

    // Controls whether the UFO is currently allowed to move.
    private bool canMove = true;

    void Update()
    {
        // Do not read movement input while movement is locked.
        if (!canMove)
            return;

        Vector2 input = Vector2.zero;

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

        Vector3 direction =
            new Vector3(input.x, input.y, 0f);

        transform.position +=
            direction.normalized *
            speed *
            Time.deltaTime;
    }

    public void LockMovement()
    {
        canMove = false;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }
}