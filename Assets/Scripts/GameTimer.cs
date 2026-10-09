using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TMP_Text timerText;

    public GameOverManager gameOverManager;

    public float gameTime = 60f;

    private float timeRemaining;
    private bool timerRunning = true;

    void Start()
    {
        timeRemaining = gameTime;
        UpdateTimerText();
    }

    void Update()
    {
        if (!timerRunning)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerRunning = false;

            GameOver();
        }

        UpdateTimerText();
    }

    void UpdateTimerText()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);

        timerText.text = "Time: " + seconds;
    }

    void GameOver()
    {
        Debug.Log("Time's up!");

        gameOverManager.ShowGameOver();
    }

    public void AddTime(float amount)
{
    timeRemaining += amount;
    UpdateTimerText();
}
}