using System.Collections;
using UnityEngine;
using TMPro;

// Displays temporary feedback messages after the
// player completes an abduction minigame attempt.
public class FeedbackManager : MonoBehaviour
{
    // TMP text used to display messages such as
    // Perfect, Good, or Miss.
    public TMP_Text feedbackText;

    // Amount of time the feedback remains visible.
    public float feedbackDuration = 1f;

    void Start()
    {
        // Hide the feedback text when the game begins.
        feedbackText.gameObject.SetActive(false);
    }

    // Displays a new feedback message.
    public void ShowFeedback(string message)
    {
        // Stop any previous feedback timer so a new message
        // can immediately replace the old one.
        StopAllCoroutines();

        StartCoroutine(
            ShowFeedbackRoutine(message)
        );
    }

    // Temporarily displays the message,
    // then hides it again after the set duration.
    IEnumerator ShowFeedbackRoutine(string message)
    {
        // Show the text and update its message.
        feedbackText.gameObject.SetActive(true);
        feedbackText.text = message;

        // Use real time so the message still works
        // independently of the game's time scale.
        yield return new WaitForSecondsRealtime(
            feedbackDuration
        );

        // Hide the message once the display time is over.
        feedbackText.gameObject.SetActive(false);
    }
}