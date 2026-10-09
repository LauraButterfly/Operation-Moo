using UnityEngine;
using TMPro;

// Controls the round timer.
// The timer starts when the opening countdown finishes,
// warns the player during the final few seconds,
// and triggers the Game Over screen when time reaches zero.
public class GameTimer : MonoBehaviour
{
    // UI text that displays the remaining time.
    public TMP_Text timerText;

    // Reference used to show the Game Over screen when time runs out.
    public GameOverManager gameOverManager;

    // Total length of one game round in seconds.
    public float gameTime = 60f;

    // Used to play the warning sound during the final seconds.
    public SFXManager sfxManager;

    // Number of seconds remaining when the warning sound should begin.
    public int warningStartTime = 5;

    // Stores the last second that triggered a warning sound.
    // This prevents the sound from playing every frame.
    private int lastWarningSecond = -1;

    // Current amount of time left in the round.
    private float timeRemaining;

    // Controls whether the timer is currently counting down.
    // It starts false so the opening countdown can finish first.
    private bool timerRunning = false;

    void Start()
    {
        // Set the timer to the full round length when the scene begins.
        timeRemaining = gameTime;

        // Display the starting time immediately.
        UpdateTimerText();
    }

    void Update()
    {
        // Do nothing while the timer is paused or has not started yet.
        if (!timerRunning)
            return;

        // Reduce the remaining time every frame.
        timeRemaining -= Time.deltaTime;

        // Round up so the displayed countdown changes at whole seconds.
        int currentSecond = Mathf.CeilToInt(timeRemaining);

        // During the final few seconds, play one warning sound per second.
        // lastWarningSecond prevents the sound from repeating every frame.
        if (currentSecond <= warningStartTime &&
            currentSecond > 0 &&
            currentSecond != lastWarningSecond)
        {
            lastWarningSecond = currentSecond;

            if (sfxManager != null)
            {
                sfxManager.PlayTimerWarningSound();
            }
        }

        // End the game when the timer reaches zero.
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerRunning = false;

            GameOver();
        }

        // Refresh the timer shown on screen.
        UpdateTimerText();
    }

    // Starts or pauses the game timer.
    // This is used by the opening countdown before gameplay begins.
    public void SetTimerRunning(bool running)
    {
        timerRunning = running;
    }

    // Updates the timer UI using the current remaining time.
    void UpdateTimerText()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);

        timerText.text = "Time: " + seconds;
    }

    // Called when the timer reaches zero.
    // Tells the GameOverManager to display the end screen.
    void GameOver()
    {
        Debug.Log("Time's up!");

        gameOverManager.ShowGameOver();
    }

    // Adds bonus time to the current round.
    // This is used when the player gets a Perfect result
    // in the abduction minigame.
    public void AddTime(float amount)
    {
        timeRemaining += amount;

        // Update the UI immediately so the player can see the bonus.
        UpdateTimerText();
    }
}