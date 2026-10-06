using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;

    public ScoreManager scoreManager;

    public AbductionMinigame minigame;

    public TractorBeam tractorBeam;

    public void ShowGameOver()
    {
        if (minigame != null)
        {
            minigame.CancelMinigame();
        }

        if (tractorBeam != null)
        {
            tractorBeam.CancelAbductionForGameOver();
        }

        gameOverPanel.SetActive(true);

        finalScoreText.text =
            "Final Score: " + scoreManager.CurrentScore;

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        // Always restore normal time before restarting.
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void QuitGame()
    {
        // This works in a built game.
        Application.Quit();

        // Lets you see that the button worked while testing in Unity.
        Debug.Log("Quit Game");
    }
}