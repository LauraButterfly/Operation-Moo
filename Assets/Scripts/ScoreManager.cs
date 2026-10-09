using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    private int score = 0;
    private int highScore = 0;

    public int CurrentScore
    {
        get { return score; }
    }

     public int HighScore
    {
        get { return highScore; }
    }

    void Start()
    {

        // Check whether a saved high score already exists.
        if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");
        }
        else
        {
            // Create the high score for the first time.
            highScore = 0;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        UpdateScoreText();
        UpdateHighScoreText();
    }

    public void AddScore(int amount)
    {
        score += amount;

        // If the current score beats the high score,
        // save the new high score.
        if (score > highScore)
        {
            highScore = score;

            PlayerPrefs.SetInt("HighScore", highScore);

            UpdateHighScoreText();
        }
        
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    void UpdateHighScoreText()
    {
        highScoreText.text = "High Score: " + highScore;
    }
}