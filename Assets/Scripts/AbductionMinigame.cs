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
    private int direction = 1;

    public void StartMinigame()
    {
        gameObject.SetActive(true);

        isRunning = true;
        direction = 1;

        marker.anchoredPosition = new Vector2(
            -bar.rect.width / 2f,
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

        Debug.Log("Minigame running");

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
    Vector2 position = marker.anchoredPosition;

    position.x += direction * markerSpeed * Time.unscaledDeltaTime;

    float halfBarWidth = bar.rect.width / 2f;

    if (position.x >= halfBarWidth)
    {
        position.x = halfBarWidth;
        direction = -1;
    }
    else if (position.x <= -halfBarWidth)
    {
        position.x = -halfBarWidth;
        direction = 1;
    }

    marker.anchoredPosition = position;
}

    void CheckResult()
    {
        isRunning = false;

        if (IsMarkerInside(greenZone))
        {
            Debug.Log("PERFECT!");

            EndMinigame();
            tractorBeam.MinigameSuccess();
        }
        else if (IsMarkerInside(orangeZone))
        {
            Debug.Log("GOOD!");

            EndMinigame();
            tractorBeam.MinigameSuccess();
        }
        else
        {
            Debug.Log("MISS!");

            EndMinigame();
            tractorBeam.MinigameFailed();
        }
    }

    bool IsMarkerInside(RectTransform zone)
    {
        float markerX = marker.anchoredPosition.x;

        float zoneCenter = zone.anchoredPosition.x;
        float halfZoneWidth = zone.rect.width / 2f;

        float leftEdge = zoneCenter - halfZoneWidth;
        float rightEdge = zoneCenter + halfZoneWidth;

        return markerX >= leftEdge && markerX <= rightEdge;
    }
}