using UnityEngine;
using UnityEngine.InputSystem;

public class AbductionMinigame : MonoBehaviour
{
    public RectTransform marker;
    public RectTransform bar;
    public RectTransform greenZone;
    public RectTransform orangeZone;

    public TractorBeam tractorBeam;

    public float markerSpeed = 400f;

    private bool isRunning;
    private float markerTime = 0f;

    public void StartMinigame()
    {
        gameObject.SetActive(true);

        isRunning = true;
        markerTime = 0f;

        float leftBound = -bar.rect.width / 2f;

        marker.anchoredPosition = new Vector2(
            leftBound,
            marker.anchoredPosition.y
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

        // Moves from 0 -> 1 -> 0 -> 1 continuously.
        float t = Mathf.PingPong(markerTime, 1f);

        // Converts that 0-1 value into a position across the bar.
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
        // Stop the marker immediately after one click.
        isRunning = false;

        if (IsMarkerInside(greenZone))
        {
            Debug.Log("PERFECT - GREEN");

            EndMinigame();
            tractorBeam.MinigameSuccess();
        }
        else if (IsMarkerInside(orangeZone))
        {
            Debug.Log("GOOD - ORANGE");

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
        Vector3[] corners = new Vector3[4];
        zone.GetWorldCorners(corners);

        float leftEdge = corners[0].x;
        float rightEdge = corners[2].x;

        float markerX = marker.position.x;

        return markerX >= leftEdge && markerX <= rightEdge;
    }
}