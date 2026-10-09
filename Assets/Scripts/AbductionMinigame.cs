using UnityEngine;
using UnityEngine.InputSystem;

// Controls the timing-based abduction minigame.
// The player stops a moving marker and receives a result
// depending on whether it lands in the green, orange, or outside area.
public class AbductionMinigame : MonoBehaviour
{
    // UI elements used by the timing bar.
    public RectTransform marker;
    public RectTransform bar;
    public RectTransform greenZone;
    public RectTransform orangeZone;

    // References to other game systems used by the minigame.
    public TractorBeam tractorBeam;
    public ScoreManager scoreManager;
    public FeedbackManager feedbackManager;
    public GameTimer gameTimer;

    // Controls how quickly the marker moves across the bar.
    public float markerSpeed = 200f;

    // Tracks whether the minigame is currently active.
    private bool isRunning;

    // Tracks the marker's movement over time.
    private float markerTime = 0f;

    // Starts the minigame and resets all values for a new attempt.
    public void StartMinigame()
    {
        // Make the minigame UI visible.
        gameObject.SetActive(true);

        isRunning = true;
        markerTime = 0f;

        // Calculate half of the total bar width.
        float halfBarWidth = bar.rect.width / 2f;

        // Reset the marker to the far-left side of the bar.
        marker.anchoredPosition = new Vector2(
            -halfBarWidth,
            marker.anchoredPosition.y
        );

        // Use the orange zone because it is the larger target area.
        float halfOrangeWidth =
            orangeZone.rect.width / 2f;

        // Choose a random horizontal position for the target.
        // The calculation keeps the entire orange zone inside the bar.
        float randomX = Random.Range(
            -halfBarWidth + halfOrangeWidth,
            halfBarWidth - halfOrangeWidth
        );

        // Move the orange and green zones together
        // so the green zone remains centered inside the orange zone.
        orangeZone.anchoredPosition = new Vector2(
            randomX,
            orangeZone.anchoredPosition.y
        );

        greenZone.anchoredPosition = new Vector2(
            randomX,
            greenZone.anchoredPosition.y
        );
    }

    // Ends the minigame and hides its UI.
    public void EndMinigame()
    {
        isRunning = false;
        gameObject.SetActive(false);
    }

    void Update()
    {
        // Do nothing unless the minigame is currently active.
        if (!isRunning)
            return;

        // Continuously move the marker across the bar.
        MoveMarker();

        // The player can stop the marker using either
        // the left mouse button or the Space key.
        bool mouseClicked =
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame;

        bool spacePressed =
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame;

        if (mouseClicked || spacePressed)
        {
            CheckResult();
        }
    }

    // Moves the marker back and forth across the full width of the bar.
    void MoveMarker()
    {
        float halfBarWidth =
            bar.rect.width / 2f;

        float leftBound = -halfBarWidth;
        float rightBound = halfBarWidth;

        // Increase markerTime based on speed and bar width.
        // unscaledDeltaTime is used so the marker is independent
        // from the normal game time scale.
        markerTime +=
            Time.unscaledDeltaTime *
            markerSpeed /
            bar.rect.width;

        // PingPong creates a repeating value:
        // 0 -> 1 -> 0 -> 1...
        float t =
            Mathf.PingPong(markerTime, 1f);

        // Convert the PingPong value into a position
        // between the left and right edges of the bar.
        float xPosition = Mathf.Lerp(
            leftBound,
            rightBound,
            t
        );

        marker.anchoredPosition = new Vector2(
            xPosition,
            marker.anchoredPosition.y
        );
    }

    // Checks where the marker stopped and applies the correct result.
    void CheckResult()
    {
        // Prevent the player from submitting more than one result.
        isRunning = false;

        // Green zone = Perfect result.
        if (IsMarkerInside(greenZone))
        {
            Debug.Log("PERFECT - GREEN");

            // Award the highest score.
            scoreManager.AddScore(100);

            // Reward a Perfect hit with 3 bonus seconds.
            gameTimer.AddTime(3f);

            // Show visual feedback to the player.
            feedbackManager.ShowFeedback(
                "PERFECT!\n+100 POINTS\n+3 SECONDS"
            );

            EndMinigame();
            tractorBeam.MinigameSuccess();
        }

        // Orange zone = Good result.
        else if (IsMarkerInside(orangeZone))
        {
            Debug.Log("GOOD - ORANGE");

            // Award a smaller amount of points.
            scoreManager.AddScore(50);

            feedbackManager.ShowFeedback(
                "GOOD!\n+50 POINTS"
            );

            EndMinigame();
            tractorBeam.MinigameSuccess();
        }

        // Outside both target zones = Miss.
        else
        {
            Debug.Log("MISS - OUTSIDE TARGET");

            feedbackManager.ShowFeedback(
                "MISS!"
            );

            EndMinigame();
            tractorBeam.MinigameFailed();
        }
    }

    // Checks whether the marker overlaps a specified target zone.
    // World-space corners are used so the check still works correctly
    // with the UI's RectTransform positions and scaling.
    bool IsMarkerInside(RectTransform zone)
    {
        Vector3[] markerCorners = new Vector3[4];
        marker.GetWorldCorners(markerCorners);

        Vector3[] zoneCorners = new Vector3[4];
        zone.GetWorldCorners(zoneCorners);

        // Get the left and right edges of the marker.
        float markerLeft = markerCorners[0].x;
        float markerRight = markerCorners[2].x;

        // Get the left and right edges of the target zone.
        float zoneLeft = zoneCorners[0].x;
        float zoneRight = zoneCorners[2].x;

        // Return true if the marker overlaps the target horizontally.
        return markerRight >= zoneLeft &&
               markerLeft <= zoneRight;
    }

    // Immediately stops and hides the minigame.
    // Used when gameplay must end, such as when the timer reaches zero.
    public void CancelMinigame()
    {
        isRunning = false;
        gameObject.SetActive(false);
    }
}