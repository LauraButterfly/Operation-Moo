using System.Collections;
using UnityEngine;
using TMPro;

public class FeedbackManager : MonoBehaviour
{
    public TMP_Text feedbackText;
    public float feedbackDuration = 1f;

    void Start()
    {
        feedbackText.gameObject.SetActive(false);
    }

    public void ShowFeedback(string message)
    {
        StopAllCoroutines();
        StartCoroutine(ShowFeedbackRoutine(message));
    }

    IEnumerator ShowFeedbackRoutine(string message)
    {
        feedbackText.gameObject.SetActive(true);
        feedbackText.text = message;

        yield return new WaitForSecondsRealtime(feedbackDuration);

        feedbackText.gameObject.SetActive(false);
    }
}