using System.Collections;
using UnityEngine;
using TMPro;

public class GameStartCountdown : MonoBehaviour
{
    public TMP_Text countdownText;

    public UfoMovement ufoMovement;
    public GameTimer gameTimer;

    public float countdownStepDuration = 1f;

    void Start()
    {
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        // Stop the player from moving during the countdown.
        if (ufoMovement != null)
        {
            ufoMovement.LockMovement();
        }

        // Stop the game timer from counting down.
        if (gameTimer != null)
        {
            gameTimer.SetTimerRunning(false);
        }

        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSecondsRealtime(countdownStepDuration);

        countdownText.text = "2";
        yield return new WaitForSecondsRealtime(countdownStepDuration);

        countdownText.text = "1";
        yield return new WaitForSecondsRealtime(countdownStepDuration);

        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(0.7f);

        countdownText.gameObject.SetActive(false);

        // Start gameplay.
        if (ufoMovement != null)
        {
            ufoMovement.UnlockMovement();
        }

        if (gameTimer != null)
        {
            gameTimer.SetTimerRunning(true);
        }
    }
}