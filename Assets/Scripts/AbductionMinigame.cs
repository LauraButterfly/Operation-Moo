using UnityEngine;
using UnityEngine.InputSystem;

public class AbductionMinigame : MonoBehaviour
{
    public RectTransform marker;
    public RectTransform bar;
    public RectTransform greenZone;
    public RectTransform orangeZone;

    public TractorBeam tractorBeam;
    public ScoreManager scoreManager;

    public float markerSpeed = 200f;

    private bool isRunning;
    private float markerTime = 0f;

    public void StartMinigame()
    {
        gameObject.SetActive(true);

        isRunning = true;
        markerTime = 0f;

        // Get the width of the full bar.
        float halfBarWidth = bar.rect.width / 2f;

        // Reset the marker to the far left.
        marker.anchoredPosition = new Vector2(
            -halfBarWidth,
            marker.anchoredPosition.y
        );

        // Use the orange zone because it is the larger target area.
        float halfOrangeWidth = orangeZone.rect.width / 2f;

        // Pick a random position while keeping the whole orange zone
        // inside the bar.
        float randomX = Random.Range(
            -halfBarWidth + halfOrangeWidth,
            halfBarWidth - halfOrangeWidth
        );

        // Move the orange and green zones together.
        orangeZone.anchoredPosition = new Vector2(
            randomX,
            orangeZone.anchoredPosition.y
        );

        greenZone.anchoredPosition = new Vector2(
            randomX,
            greenZone.anchoredPosition.y
        );
    }

    public void EndMinigame()
    {
        isRunning = false;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isRunning)
            return;

        MoveMarker();

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

    void MoveMarker()
    {
        float halfBarWidth = bar.rect.width / 2f;

        float leftBound = -halfBarWidth;
        float rightBound = halfBarWidth;

        markerTime +=
            Time.unscaledDeltaTime *
            markerSpeed /
            bar.rect.width;

        // Produces a repeating value:
        // 0 -> 1 -> 0 -> 1...
        float t = Mathf.PingPong(markerTime, 1f);

        // Move smoothly from the left side to the right side.
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

    void CheckResult()
    {
        // Stop accepting input immediately.
        isRunning = false;

        if (IsMarkerInside(greenZone))
        {
            Debug.Log("PERFECT - GREEN");

            // Full points for green.
            scoreManager.AddScore(100);

            EndMinigame();
            tractorBeam.MinigameSuccess();
        }
        else if (IsMarkerInside(orangeZone))
        {
            Debug.Log("GOOD - ORANGE");

            // Reduced points for orange.
            scoreManager.AddScore(50);

            EndMinigame();
            tractorBeam.MinigameSuccess();
        }
        else
        {
            Debug.Log("MISS - OUTSIDE TARGET");

            // No points for a miss.
            EndMinigame();
            tractorBeam.MinigameFailed();
        }
    }

    bool IsMarkerInside(RectTransform zone)
    {
        // Get the actual visible world-space corners of the zone.
        Vector3[] corners = new Vector3[4];
        zone.GetWorldCorners(corners);

        float leftEdge = corners[0].x;
        float rightEdge = corners[2].x;

        float markerX = marker.position.x;

        return markerX >= leftEdge &&
               markerX <= rightEdge;
    }

    public void CancelMinigame()
    {
        isRunning = false;
        gameObject.SetActive(false);
    }
}