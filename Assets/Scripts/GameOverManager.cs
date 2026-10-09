using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Controls the Game Over screen.
// It stops active gameplay systems, displays the final score
// and high score, and handles the Game Over menu buttons.
public class GameOverManager : MonoBehaviour
{
    // Panel that becomes visible when the game ends.
    public GameObject gameOverPanel;

    // Text fields used to display the player's results.
    public TMP_Text finalScoreText;
    public TMP_Text finalHighScoreText;

    // Reference to the score system.
    public ScoreManager scoreManager;

    // Reference to the minigame so it can be stopped
    // if the timer reaches zero during an attempt.
    public AbductionMinigame minigame;

    // Reference to the tractor beam so any active
    // abduction can be cancelled when the game ends.
    public TractorBeam tractorBeam;

    // Displays the Game Over screen and stops gameplay.
    public void ShowGameOver()
    {
        // Stop and hide the minigame if it is currently active.
        if (minigame != null)
        {
            minigame.CancelMinigame();
        }

        // Stop any cow abduction that is currently in progress.
        if (tractorBeam != null)
        {
            tractorBeam.CancelAbductionForGameOver();
        }

        // Show the Game Over menu.
        gameOverPanel.SetActive(true);

        // Display the player's final score.
        finalScoreText.text =
            "Final Score: " + scoreManager.CurrentScore;

        // Display the saved high score.
        finalHighScoreText.text =
            "High Score: " + scoreManager.HighScore;

        // Pause the game while the Game Over menu is open.
        Time.timeScale = 0f;
    }

    // Restarts the current game scene.
    public void RestartGame()
    {
        // Restore normal time before reloading the scene.
        Time.timeScale = 1f;

        // Reload the currently active scene.
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // Returns the player to the Main Menu scene.
    public void ReturnToMainMenu()
    {
        // Restore normal time before changing scenes.
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }

    // Closes the application.
    public void QuitGame()
    {
        // Application.Quit only closes the game
        // when running as a built application.
        Application.Quit();

        // This message confirms that the button was pressed
        // while testing inside the Unity Editor.
        Debug.Log("Quit Game");
    }
}