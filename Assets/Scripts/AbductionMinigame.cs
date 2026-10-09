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

    public GameTimer gameTimer;

    public float markerSpeed = 200f;

    private bool isRunning;
    private float markerTime = 0f;

    public void StartMinigame()
    {
        gameObject.SetActive(true);

        isRunning = true;
        markerTime = 0f;

        // Get half of the full bar width.
        float halfBarWidth = bar.rect.width / 2f;

        // Reset marker to the far left.
        marker.anchoredPosition = new Vector2(
            -halfBarWidth,
            marker.anchoredPosition.y
        );

        // Use the orange zone because it is the larger target.
        float halfOrangeWidth =
            orangeZone.rect.width / 2f;

        // Choose a random target location while keeping the
        // entire orange zone inside the bar.
        float randomX = Random.Range(
            -halfBarWidth + halfOrangeWidth,
            halfBarWidth - halfOrangeWidth
        );

        // Move both target zones together.
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
        float halfBarWidth =
            bar.rect.width / 2f;

        float leftBound = -halfBarWidth;
        float rightBound = halfBarWidth;

        markerTime +=
            Time.unscaledDeltaTime *
            markerSpeed /
            bar.rect.width;

        // Creates:
        // 0 -> 1 -> 0 -> 1...
        float t =
            Mathf.PingPong(markerTime, 1f);

        // Smoothly moves between the left and right edges.
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
        // Only allow one attempt.
        isRunning = false;

        if (IsMarkerInside(greenZone))
        {
            Debug.Log("PERFECT - GREEN");

            scoreManager.AddScore(100);

            //Add 5 seconds to the timer ofr perfect hit
            gameTimer.AddTime(5f);

            EndMinigame();
            tractorBeam.MinigameSuccess();
        }
        else if (IsMarkerInside(orangeZone))
        {
            Debug.Log("GOOD - ORANGE");

            scoreManager.AddScore(50);

            EndMinigame();
            tractorBeam.MinigameSuccess();
        }
        else
        {
            Debug.Log("MISS - OUTSIDE TARGET");

            EndMinigame();
            tractorBeam.MinigameFailed();
        }
    }

    bool IsMarkerInside(RectTransform zone)
{
    Vector3[] markerCorners = new Vector3[4];
    marker.GetWorldCorners(markerCorners);

    Vector3[] zoneCorners = new Vector3[4];
    zone.GetWorldCorners(zoneCorners);

    float markerLeft = markerCorners[0].x;
    float markerRight = markerCorners[2].x;

    float zoneLeft = zoneCorners[0].x;
    float zoneRight = zoneCorners[2].x;

    return markerRight >= zoneLeft &&
           markerLeft <= zoneRight;
}

    public void CancelMinigame()
    {
        isRunning = false;
        gameObject.SetActive(false);
    }
}