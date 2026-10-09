using UnityEngine;

// Makes the camera smoothly follow a target,
// which is normally the player's UFO.
public class CameraFollow : MonoBehaviour
{
    // Object the camera should follow.
    public Transform target;

    // Controls how quickly the camera catches up to the target.
    // Higher values make the camera follow more closely.
    public float smoothSpeed = 5f;

    // Stores the camera's starting distance from the target.
    // This keeps the same relative camera position during gameplay.
    private Vector3 offset;

    void Start()
    {
        // Calculate and store the starting offset between
        // the camera and the object it is following.
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        // Calculate where the camera should be based on
        // the target's current position and the original offset.
        Vector3 targetPosition = target.position + offset;

        // Smoothly move the camera toward the desired position.
        // LateUpdate is used so the target finishes moving first,
        // which helps prevent jitter.
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}