using UnityEngine;
using UnityEngine.InputSystem;

// Controls the UFO's tractor beam and the full cow abduction process.
// The player presses E while a cow is inside the beam area to start
// the minigame. A successful attempt pulls the cow into the UFO,
// while a failed attempt stuns the cow.
public class TractorBeam : MonoBehaviour
{
    // Visual effect displayed while the tractor beam is active.
    public GameObject beamVisual;

    // Reference to the timing-based abduction minigame.
    public AbductionMinigame minigame;

    // Reference to the UFO movement script so movement can be
    // locked during the minigame and abduction sequence.
    public UfoMovement ufoMovement;

    // Reference used to play abduction and failure sound effects.
    public SFXManager sfxManager;

    // Position inside the UFO that the cow moves toward
    // after a successful minigame attempt.
    public Transform abductionTarget;

    // Speed at which the cow moves toward the UFO.
    public float abductionSpeed = 2f;

    // Final scale multiplier applied to the cow
    // as it gets closer to the UFO.
    public float shrinkAmount = 0.5f;

    // Stores the cow's original size before it begins shrinking.
    private Vector3 originalCowScale;

    // True while a successful cow is physically
    // being pulled toward the UFO.
    private bool isAbducting = false;

    // Stores the cow currently inside and selected by the beam area.
    private CowBehavior cowInBeam;

    // Starting distance between the cow and the UFO.
    // This is used to calculate the cow's shrinking progress.
    private float abductionStartDistance;

    // Tracks whether the timing minigame is currently active.
    private bool minigameActive = false;

    void Update()
    {
        // Stop checking for input if no keyboard is available.
        if (Keyboard.current == null)
            return;

        // Press E to activate the tractor beam.
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ActivateBeam();
        }
    }

    void FixedUpdate()
    {
        // Only move the cow after the player successfully
        // completes the minigame.
        if (!isAbducting || cowInBeam == null)
            return;

        // Move the cow toward the abduction target inside the UFO.
        cowInBeam.transform.position = Vector3.MoveTowards(
            cowInBeam.transform.position,
            abductionTarget.position,
            abductionSpeed * Time.fixedDeltaTime
        );

        // Measure how far the cow still is from the UFO.
        float distance = Vector2.Distance(
            cowInBeam.transform.position,
            abductionTarget.position
        );

        // Calculate abduction progress from 0 to 1.
        // The value approaches 1 as the cow gets closer to the UFO.
        float progress = 1f - Mathf.Clamp01(
            distance / abductionStartDistance
        );

        // Calculate the cow's final smaller size.
        Vector3 targetScale =
            originalCowScale * shrinkAmount;

        // Gradually shrink the cow based on how close
        // it is to the UFO.
        cowInBeam.transform.localScale = Vector3.Lerp(
            originalCowScale,
            targetScale,
            progress
        );

        // Complete the abduction once the cow is
        // close enough to the target position.
        if (distance < 0.1f)
        {
            CompleteAbduction();
        }
    }

    // Called when another collider first enters
    // the tractor beam's trigger area.
    void OnTriggerEnter2D(Collider2D other)
    {
        CowBehavior cow =
            other.GetComponent<CowBehavior>();

        // Only allow cows that are not currently stunned
        // to become the active abduction target.
        if (cow != null && !cow.IsStunned)
        {
            cowInBeam = cow;

            Debug.Log("Cow entered beam area!");
        }
    }

    // Called continuously while another collider
    // remains inside the tractor beam area.
    void OnTriggerStay2D(Collider2D other)
    {
        CowBehavior cow =
            other.GetComponent<CowBehavior>();

        // Ignore objects that are not cows.
        if (cow == null)
            return;

        // A stunned cow cannot be selected for another abduction.
        if (cow.IsStunned)
        {
            if (cow == cowInBeam)
            {
                cowInBeam = null;
            }

            return;
        }

        // If a cow's stun ends while it is still inside the beam,
        // allow it to become selectable again.
        if (cowInBeam == null &&
            !minigameActive &&
            !isAbducting)
        {
            cowInBeam = cow;
        }
    }

    // Called when a collider leaves the tractor beam area.
    void OnTriggerExit2D(Collider2D other)
    {
        CowBehavior cow =
            other.GetComponent<CowBehavior>();

        if (cow != null && cow == cowInBeam)
        {
            // Keep the cow selected if the minigame or
            // physical abduction is already in progress.
            if (!minigameActive && !isAbducting)
            {
                cowInBeam = null;
            }
        }
    }

    // Attempts to begin an abduction when the player presses E.
    void ActivateBeam()
    {
        // Only activate if there is a valid cow available
        // and another abduction is not already in progress.
        if (cowInBeam != null &&
            !cowInBeam.IsStunned &&
            !isAbducting &&
            !minigameActive)
        {
            // Prevent the UFO from moving during the minigame.
            if (ufoMovement != null)
            {
                ufoMovement.LockMovement();
            }

            // Show the tractor beam effect.
            beamVisual.SetActive(true);

            // Save the cow's original scale before
            // it begins shrinking.
            originalCowScale =
                cowInBeam.transform.localScale;

            // Stop the cow's normal movement and animation.
            cowInBeam.StartAbduction();

            // Start the timing minigame.
            minigameActive = true;
            minigame.StartMinigame();
        }
    }

    // Called by the minigame when the player gets
    // either a Perfect or Good result.
    public void MinigameSuccess()
    {
        minigameActive = false;

        // Play the abduction sound effect.
        if (sfxManager != null)
        {
            sfxManager.PlayAbductionSound();
        }

        // Safety check in case the selected cow
        // was lost unexpectedly.
        if (cowInBeam == null)
        {
            Debug.LogError(
                "Minigame succeeded, but there is no cow assigned!"
            );

            if (ufoMovement != null)
            {
                ufoMovement.UnlockMovement();
            }

            return;
        }

        // Record the starting distance so the cow's
        // shrinking animation can be calculated correctly.
        abductionStartDistance = Vector2.Distance(
            cowInBeam.transform.position,
            abductionTarget.position
        );

        // Begin physically pulling the cow toward the UFO.
        isAbducting = true;

        // UFO movement stays locked until the
        // entire abduction animation is complete.
    }

    // Called by the minigame when the player misses the target.
    public void MinigameFailed()
    {
        minigameActive = false;

        // Turn off the tractor beam visual.
        beamVisual.SetActive(false);

        // Play the failure sound effect.
        if (sfxManager != null)
        {
            sfxManager.PlayFailSound();
        }

        // Stun the cow so it cannot immediately
        // be targeted again.
        if (cowInBeam != null)
        {
            cowInBeam.Stun();
        }

        // Clear the current target.
        cowInBeam = null;

        // Allow the player to move again after the failed attempt.
        if (ufoMovement != null)
        {
            ufoMovement.UnlockMovement();
        }
    }

    // Finishes the abduction once the cow reaches the UFO.
    void CompleteAbduction()
    {
        Debug.Log("Cow successfully abducted!");

        // Stop the abduction sound when the cow
        // reaches the UFO.
        if (sfxManager != null)
        {
            sfxManager.StopAbductionSound();
        }

        // Remove the successfully abducted cow from the scene.
        Destroy(cowInBeam.gameObject);

        // Reset the abduction state.
        cowInBeam = null;
        isAbducting = false;

        // Hide the tractor beam visual.
        beamVisual.SetActive(false);

        // Allow the player to move again.
        if (ufoMovement != null)
        {
            ufoMovement.UnlockMovement();
        }
    }

    // Cancels any active abduction when the game timer reaches zero.
    public void CancelAbductionForGameOver()
    {
        minigameActive = false;
        isAbducting = false;

        // Hide the tractor beam when the game ends.
        beamVisual.SetActive(false);

        // Movement is intentionally not unlocked here
        // because gameplay has already ended.
    }
}