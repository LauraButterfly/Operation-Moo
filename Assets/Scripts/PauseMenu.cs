using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

// Controls pausing and resuming the game.
// Pressing Escape opens or closes the pause menu.
// While paused, gameplay is stopped and the abduction minigame is disabled.
public class PauseMenu : MonoBehaviour
{
    // UI panel that contains the pause menu buttons.
    public GameObject pausePanel;

    // Reference to the abduction minigame so it can be disabled while paused.
    public AbductionMinigame minigame;

    // Tracks whether the game is currently paused.
    private bool isPaused = false;

    void Update()
    {
        // Stop checking for input if no keyboard is available.
        if (Keyboard.current == null)
            return;

        // Press Escape to toggle between paused and unpaused states.
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Pauses the game and displays the pause menu.
    public void PauseGame()
    {
        isPaused = true;

        // Show the pause menu UI.
        pausePanel.SetActive(true);

        // Disable the minigame so its marker does not continue moving
        // while the rest of the game is paused.
        if (minigame != null)
        {
            minigame.enabled = false;
        }

        // Stop all gameplay that uses scaled time.
        Time.timeScale = 0f;
    }

    // Resumes gameplay and hides the pause menu.
    public void ResumeGame()
    {
        isPaused = false;

        // Hide the pause menu UI.
        pausePanel.SetActive(false);

        // Re-enable the minigame if it was active before pausing.
        if (minigame != null)
        {
            minigame.enabled = true;
        }

        // Restore normal game speed.
        Time.timeScale = 1f;
    }

    // Returns the player to the Main Menu scene.
    public void ReturnToMainMenu()
    {
        // Make sure time is restored before changing scenes.
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}