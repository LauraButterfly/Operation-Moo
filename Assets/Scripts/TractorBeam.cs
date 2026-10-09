using UnityEngine;
using UnityEngine.InputSystem;

// Controls the UFO tractor beam.
// Press E while a cow is inside the beam area to start the minigame.
// If the player succeeds, the cow is pulled into the UFO and shrinks.
// The UFO stays locked until the whole abduction sequence is finished.
public class TractorBeam : MonoBehaviour
{
    // Visual effect shown while the beam is active.
    public GameObject beamVisual;

    // Reference to the abduction minigame.
    public AbductionMinigame minigame;

    // Reference to the UFO movement script.
    public UfoMovement ufoMovement;

    // Reference to the SFX manager.
    public SFXManager sfxManager;

    // Final point the cow moves toward.
    public Transform abductionTarget;

    // How fast the cow moves toward the UFO.
    public float abductionSpeed = 2f;

    // Final size multiplier of the cow during abduction.
    public float shrinkAmount = 0.5f;

    // Stores the cow's original scale.
    private Vector3 originalCowScale;

    // True while the cow is physically being pulled into the UFO.
    private bool isAbducting = false;

    // Cow currently selected by the beam.
    private CowBehavior cowInBeam;

    // Starting distance used for calculating shrink progress.
    private float abductionStartDistance;

    // True while the timing minigame is active.
    private bool minigameActive = false;

    void Update()
    {
        if (Keyboard.current == null)
            return;

        // E activates the tractor beam.
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ActivateBeam();
        }
    }

    void FixedUpdate()
    {
        // Only move the cow after the player succeeds at the minigame.
        if (!isAbducting || cowInBeam == null)
            return;

        // Move the cow toward the UFO.
        cowInBeam.transform.position = Vector3.MoveTowards(
            cowInBeam.transform.position,
            abductionTarget.position,
            abductionSpeed * Time.fixedDeltaTime
        );

        float distance = Vector2.Distance(
            cowInBeam.transform.position,
            abductionTarget.position
        );

        // Goes from 0 to 1 as the cow approaches the UFO.
        float progress = 1f - Mathf.Clamp01(
            distance / abductionStartDistance
        );

        // Shrink the cow during the abduction.
        Vector3 targetScale =
            originalCowScale * shrinkAmount;

        cowInBeam.transform.localScale = Vector3.Lerp(
            originalCowScale,
            targetScale,
            progress
        );

        // Finish when the cow reaches the UFO.
        if (distance < 0.1f)
        {
            CompleteAbduction();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        CowBehavior cow =
            other.GetComponent<CowBehavior>();

        // Only select cows that are not stunned.
        if (cow != null && !cow.IsStunned)
        {
            cowInBeam = cow;
            Debug.Log("Cow entered beam area!");
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        CowBehavior cow =
            other.GetComponent<CowBehavior>();

        if (cow == null)
            return;

        // If the cow is stunned, make sure it cannot stay selected.
        if (cow.IsStunned)
        {
            if (cow == cowInBeam)
            {
                cowInBeam = null;
            }

            return;
        }

        // If the stun has ended while the cow is still inside the beam area,
        // allow it to become selectable again.
        if (cowInBeam == null &&
            !minigameActive &&
            !isAbducting)
        {
            cowInBeam = cow;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        CowBehavior cow =
            other.GetComponent<CowBehavior>();

        if (cow != null && cow == cowInBeam)
        {
            // Keep the cow selected while the minigame
            // or abduction animation is active.
            if (!minigameActive && !isAbducting)
            {
                cowInBeam = null;
            }
        }
    }

    void ActivateBeam()
    {
        if (cowInBeam != null &&
            !cowInBeam.IsStunned &&
            !isAbducting &&
            !minigameActive)
        {
            // Lock UFO movement immediately.
            if (ufoMovement != null)
            {
                ufoMovement.LockMovement();
            }

            beamVisual.SetActive(true);

            originalCowScale =
                cowInBeam.transform.localScale;

            // Freeze the cow.
            cowInBeam.StartAbduction();

            // Start the minigame.
            minigameActive = true;
            minigame.StartMinigame();
        }
    }

    public void MinigameSuccess()
    {
        minigameActive = false;

        // Play the successful abduction sound.
        if (sfxManager != null)
        {
            sfxManager.PlayAbductionSound();
        }

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

        abductionStartDistance = Vector2.Distance(
            cowInBeam.transform.position,
            abductionTarget.position
        );

        // Start the physical abduction.
        isAbducting = true;

        // Do NOT unlock UFO movement here.
    }

    public void MinigameFailed()
    {
        minigameActive = false;

        beamVisual.SetActive(false);

        // Play the failure sound.
        if (sfxManager != null)
        {
            sfxManager.PlayFailSound();
        }

        if (cowInBeam != null)
        {
            cowInBeam.Stun();
        }

        cowInBeam = null;

        if (ufoMovement != null)
        {
            ufoMovement.UnlockMovement();
        }
    }

    void CompleteAbduction()
    {
        Debug.Log("Cow successfully abducted!");

        // Stop aduction sound when the cow reaches the UFO.
        if (sfxManager != null)
        {
            sfxManager.StopAbductionSound();
        }

        Destroy(cowInBeam.gameObject);

        cowInBeam = null;
        isAbducting = false;

        beamVisual.SetActive(false);

        if (ufoMovement != null)
        {
            ufoMovement.UnlockMovement();
        }
    }

    public void CancelAbductionForGameOver()
    {
        minigameActive = false;
        isAbducting = false;

        beamVisual.SetActive(false);

        // Do not unlock movement here,
        // because the game is already over.
    }
}