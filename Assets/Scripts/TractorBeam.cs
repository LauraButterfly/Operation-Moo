using UnityEngine;
using UnityEngine.InputSystem;

// This script controls the UFO tractor beam effect.
// When a cow enters the trigger zone and the player presses Space,
// the cow is pulled toward an abduction target, shrinks, and is then destroyed.
public class TractorBeam : MonoBehaviour
{
    // Visual effect that is enabled while the beam is active.
    public GameObject beamVisual;

    // The final destination the cow is pulled toward.
    public Transform abductionTarget;

    // How fast the cow moves toward the target.
    public float abductionSpeed = 2f;

    // How much the cow shrinks while being abducted.
    public float shrinkAmount = 0.5f;

    // Stores the cow's original scale so it can be smoothly reduced during the beam.
    private Vector3 originalCowScale;

    // Tracks whether the beam is currently pulling a cow.
    private bool isAbducting = false;

    // Reference to the current cow being abducted.
    private CowBehavior cowInBeam;

    // Distance from the cow to the target when the abduction begins.
    private float abductionStartDistance;

    void Update()
    {
        // Ignore input if the keyboard system is not available.
        if (Keyboard.current == null)
            return;

        // Pressing Space starts the abduction if a cow is in range.
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ActivateBeam();
        }
    }

    void FixedUpdate()
    {
        // Only update movement while an abduction is active and a valid cow exists.
        if (!isAbducting || cowInBeam == null)
            return;

        // Move the cow steadily toward the abduction target.
        cowInBeam.transform.position = Vector3.MoveTowards(
            cowInBeam.transform.position,
            abductionTarget.position,
            abductionSpeed * Time.fixedDeltaTime
        );

        // Measure how close the cow is to the target.
        float distance = Vector2.Distance(
            cowInBeam.transform.position,
            abductionTarget.position
        );

        // progress goes from 0 to 1 as the cow nears the target.
        float progress = 1f - Mathf.Clamp01(
            distance / abductionStartDistance
        );

        // The cow shrinks from its original size toward the target size.
        Vector3 targetScale = originalCowScale * shrinkAmount;

        cowInBeam.transform.localScale = Vector3.Lerp(
            originalCowScale,
            targetScale,
            progress
        );

        // When the cow gets very close, finish the abduction.
        if (distance < 0.1f)
        {
            CompleteAbduction();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check whether the object entering the trigger is a cow.
        CowBehavior cow = other.GetComponent<CowBehavior>();

        if (cow != null)
        {
            // Store the cow so the beam knows what to abduct.
            cowInBeam = cow;
            Debug.Log("Cow entered beam area!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // If the cow leaves the beam, it is no longer the active target.
        CowBehavior cow = other.GetComponent<CowBehavior>();

        if (cow != null && cow == cowInBeam)
        {
            cowInBeam = null;
        }
    }

    void ActivateBeam()
    {
        // Only start if a cow is present and the beam is not already active.
        if (cowInBeam != null && !isAbducting)
        {
            isAbducting = true;

            // Enable the beam effect while the cow is being pulled in.
            beamVisual.SetActive(true);

            // Save the cow's current size before shrinking it.
            originalCowScale = cowInBeam.transform.localScale;

            // Record the starting distance so the shrink animation can be based on progress.
            abductionStartDistance = Vector2.Distance(
                cowInBeam.transform.position,
                abductionTarget.position
            );

            // Tell the cow script that the abduction sequence has started.
            cowInBeam.StartAbduction();

            Debug.Log("Starting cow abduction!");
        }
    }

    void CompleteAbduction()
    {
        Debug.Log("Cow successfully abducted!");

        // Remove the cow object from the scene once it reaches the target.
        Destroy(cowInBeam.gameObject);

        // Reset all state so the beam is ready for the next cow.
        cowInBeam = null;
        isAbducting = false;

        beamVisual.SetActive(false);
    }
}