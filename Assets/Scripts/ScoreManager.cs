using UnityEngine;
using TMPro;

// Manages the player's current score and saved high score.
// The high score is stored using PlayerPrefs so it remains
// available after restarting the game.
public class ScoreManager : MonoBehaviour
{
    // UI text that displays the current score.
    public TMP_Text scoreText;

    // UI text that displays the saved high score.
    public TMP_Text highScoreText;

    // Current score for this round.
    private int score = 0;

    // Highest score saved across game sessions.
    private int highScore = 0;

    // Allows other scripts to read the current score
    // without directly changing it.
    public int CurrentScore
    {
        get { return score; }
    }

    // Allows other scripts to read the current high score
    // without directly changing it.
    public int HighScore
    {
        get { return highScore; }
    }

    void Start()
    {
        // Check whether a saved high score already exists.
        if (PlayerPrefs.HasKey("HighScore"))
        {
            // Load the previously saved high score.
            highScore = PlayerPrefs.GetInt("HighScore");
        }
        else
        {
            // Create a high score value the first time
            // the game is played.
            highScore = 0;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        // Display the starting score and saved high score.
        UpdateScoreText();
        UpdateHighScoreText();
    }

    // Adds points to the player's current score.
    public void AddScore(int amount)
    {
        score += amount;

        // If the current score is higher than the saved
        // high score, replace it with the new value.
        if (score > highScore)
        {
            highScore = score;

            // Save the new high score so it remains
            // available between game sessions.
            PlayerPrefs.SetInt("HighScore", highScore);

            // Immediately update the displayed high score.
            UpdateHighScoreText();
        }

        // Update the current score display.
        UpdateScoreText();
    }

    // Updates the current score shown on screen.
    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    // Updates the high score shown on screen.
    void UpdateHighScoreText()
    {
        highScoreText.text = "High Score: " + highScore;
    }
}