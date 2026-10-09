using UnityEngine;
using UnityEngine.SceneManagement;

// Controls the buttons and panels on the Main Menu.
// The player can start the game, open or close Settings,
// or quit the application.
public class MainMenu : MonoBehaviour
{
    // Contains the main Play, Settings, and Quit buttons.
    public GameObject mainButtons;

    // Panel containing the game's settings controls.
    public GameObject settingsPanel;

    // Starts the game by loading the main gameplay scene.
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    // Opens the Settings menu.
    public void OpenSettings()
    {
        // Hide the main menu buttons.
        mainButtons.SetActive(false);

        // Show the Settings panel.
        settingsPanel.SetActive(true);
    }

    // Closes the Settings menu and returns to
    // the normal Main Menu buttons.
    public void CloseSettings()
    {
        // Hide the Settings panel.
        settingsPanel.SetActive(false);

        // Show the main menu buttons again.
        mainButtons.SetActive(true);
    }

    // Closes the application.
    public void QuitGame()
    {
        // This message confirms that the button worked
        // while testing inside the Unity Editor.
        Debug.Log("Quit Game");

        // Application.Quit works when running
        // the game as a built application.
        Application.Quit();
    }
}