using System.Collections;
using UnityEngine;
using TMPro;

// Controls the countdown shown before gameplay begins.
// The player cannot move and the game timer does not start
// until the countdown has finished.
public class GameStartCountdown : MonoBehaviour
{
    // Text used to display 3, 2, 1, and GO.
    public TMP_Text countdownText;

    // References to gameplay systems that should remain
    // inactive during the countdown.
    public UfoMovement ufoMovement;
    public GameTimer gameTimer;

    // Amount of time each countdown number stays on screen.
    public float countdownStepDuration = 1f;

    void Start()
    {
        // Begin the countdown as soon as the game scene starts.
        StartCoroutine(CountdownRoutine());
    }

    // Handles the full opening countdown sequence.
    IEnumerator CountdownRoutine()
    {
        // Prevent the player from moving during the countdown.
        if (ufoMovement != null)
        {
            ufoMovement.LockMovement();
        }

        // Prevent the round timer from counting down
        // before gameplay officially begins.
        if (gameTimer != null)
        {
            gameTimer.SetTimerRunning(false);
        }

        // Make sure the countdown text is visible.
        countdownText.gameObject.SetActive(true);

        // Display each countdown number for the chosen duration.
        countdownText.text = "3";
        yield return new WaitForSecondsRealtime(
            countdownStepDuration
        );

        countdownText.text = "2";
        yield return new WaitForSecondsRealtime(
            countdownStepDuration
        );

        countdownText.text = "1";
        yield return new WaitForSecondsRealtime(
            countdownStepDuration
        );

        // Briefly show GO before enabling gameplay.
        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(0.7f);

        // Hide the countdown once it has finished.
        countdownText.gameObject.SetActive(false);

        // Allow the player to move.
        if (ufoMovement != null)
        {
            ufoMovement.UnlockMovement();
        }

        // Start the main game timer.
        if (gameTimer != null)
        {
            gameTimer.SetTimerRunning(true);
        }
    }
}