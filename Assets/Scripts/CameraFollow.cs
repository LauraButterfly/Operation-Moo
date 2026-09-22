using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Object the camera should follow, usually the player.
    public Transform target;
    // Higher values make the camera catch up to the target more quickly.
    public float smoothSpeed = 5f;

    // Keeps the camera at the same relative position it had at startup.
    private Vector3 offset;

    void Start()
    {
        // Store the initial spacing between the camera and its target.
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        // Calculate the camera position that preserves the original offset.
        Vector3 targetPosition = target.position + offset;

        // Move smoothly after all normal Update movement has finished.
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}