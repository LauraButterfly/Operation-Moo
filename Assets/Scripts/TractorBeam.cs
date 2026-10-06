using UnityEngine;
using UnityEngine.InputSystem;

// This script controls the UFO tractor beam effect.
// When a cow enters the trigger zone and the player presses Space,
// the cow is pulled toward an abduction target, shrinks, and is then destroyed.
public class TractorBeam : MonoBehaviour
{
    // Visual effect that is enabled while the beam is active.
    public GameObject beamVisual;

    public AbductionMinigame minigame;

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

    private bool minigameActive = false;

    void Update()
    {
        // Ignore input if the keyboard system is not available.
        if (Keyboard.current == null)
            return;


        // Pressing E starts the abduction if a cow is in range.
        if (Keyboard.current.eKey.wasPressedThisFrame)
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
    CowBehavior cow = other.GetComponent<CowBehavior>();

    if (cow != null)
    {
        cowInBeam = cow;
        Debug.Log("Cow entered beam area!");
    }
}

    void OnTriggerExit2D(Collider2D other)
    {
        CowBehavior cow = other.GetComponent<CowBehavior>();

        if (cow != null && cow == cowInBeam)
        {
            // Keep the cow locked as the target while the
            // minigame or abduction is in progress.
            if (!minigameActive && !isAbducting)
            {
                cowInBeam = null;
            }
        }
    }

    void ActivateBeam()
    {
        if (cowInBeam != null && !isAbducting && !minigameActive)
        {
            beamVisual.SetActive(true);

            originalCowScale = cowInBeam.transform.localScale;

            cowInBeam.StartAbduction();

            minigameActive = true;
            minigame.StartMinigame();
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

    public void MinigameSuccess()
    {
        minigameActive = false;

        if (cowInBeam == null)
        {
            Debug.LogError("Minigame succeeded, but there is no cow assigned!");
            return;
        }

        abductionStartDistance = Vector2.Distance(
            cowInBeam.transform.position,
            abductionTarget.position
        );

        isAbducting = true;
    }

    public void MinigameFailed()
    {
        minigameActive = false;

        beamVisual.SetActive(false);

        if (cowInBeam != null)
        {
            cowInBeam.StopAbduction();
        }

        cowInBeam = null;
    }
}