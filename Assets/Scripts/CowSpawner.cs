using System.Collections.Generic;
using UnityEngine;

// Maintains a target number of cows on the map.
// New cows are only spawned at valid spawn points that are
// off-screen, not too close to another cow, and not the last point used.
public class CowSpawner : MonoBehaviour
{
    // Cow prefab that will be created by the spawner.
    public GameObject cowPrefab;

    // Possible locations where cows are allowed to spawn.
    public Transform[] spawnPoints;

    // Desired number of cows that should be present in the scene.
    public int targetCowCount = 7;

    // Delay between spawn attempts.
    public float spawnDelay = 2f;

    // Minimum distance required between a spawn point
    // and any cow already in the scene.
    public float minimumCowDistance = 2f;

    // Counts down until the next spawn attempt.
    private float spawnTimer;

    // Stores the most recently used spawn point
    // so the same location is not immediately reused.
    private Transform lastSpawnPoint;

    void Start()
    {
        // Start the spawn timer at the full delay.
        spawnTimer = spawnDelay;
    }

    void Update()
    {
        // Count all cows currently active in the scene.
        int currentCowCount =
            FindObjectsByType<CowBehavior>(
                FindObjectsSortMode.None
            ).Length;

        // If there are already enough cows,
        // reset the spawn timer and do nothing.
        if (currentCowCount >= targetCowCount)
        {
            spawnTimer = spawnDelay;
            return;
        }

        // Count down until another spawn attempt can be made.
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnCow();

            // Reset the timer after each spawn attempt.
            spawnTimer = spawnDelay;
        }
    }

    // Attempts to spawn one cow at a valid location.
    void SpawnCow()
    {
        // Stop if no prefab or spawn points have been assigned.
        if (cowPrefab == null || spawnPoints.Length == 0)
            return;

        // Create a temporary list of spawn points
        // that meet all spawning requirements.
        List<Transform> validSpawnPoints =
            new List<Transform>();

        foreach (Transform spawnPoint in spawnPoints)
        {
            // Do not spawn somewhere currently visible to the player.
            if (IsSpawnPointVisible(spawnPoint))
                continue;

            // Avoid immediately using the same spawn point twice.
            if (spawnPoint == lastSpawnPoint)
                continue;

            // Avoid spawning a new cow too close to another cow.
            if (IsCowNearby(spawnPoint))
                continue;

            // This spawn point passed all checks and can be used.
            validSpawnPoints.Add(spawnPoint);
        }

        // If no valid locations are available,
        // wait until the next spawn attempt instead.
        if (validSpawnPoints.Count == 0)
            return;

        // Randomly choose one of the valid spawn points.
        Transform chosenSpawnPoint =
            validSpawnPoints[
                Random.Range(0, validSpawnPoints.Count)
            ];

        // Create a new cow at the chosen position.
        Instantiate(
            cowPrefab,
            chosenSpawnPoint.position,
            Quaternion.identity
        );

        // Remember this point so it cannot be used
        // again on the very next spawn.
        lastSpawnPoint = chosenSpawnPoint;
    }

    // Checks whether a spawn point is currently visible
    // inside or slightly outside the camera view.
    bool IsSpawnPointVisible(Transform spawnPoint)
    {
        // If there is no Main Camera, treat the point as not visible.
        if (Camera.main == null)
            return false;

        // Convert the world position into viewport coordinates.
        // A normal visible viewport ranges from 0 to 1 on both axes.
        Vector3 viewportPosition =
            Camera.main.WorldToViewportPoint(
                spawnPoint.position
            );

        // The extra 0.1 margin prevents cows from appearing
        // immediately at the edge of the player's screen.
        return viewportPosition.x >= -0.1f &&
               viewportPosition.x <= 1.1f &&
               viewportPosition.y >= -0.1f &&
               viewportPosition.y <= 1.1f &&
               viewportPosition.z > 0f;
    }

    // Checks whether another cow is already too close
    // to the given spawn point.
    bool IsCowNearby(Transform spawnPoint)
    {
        // Find all cows currently active in the scene.
        CowBehavior[] cows =
            FindObjectsByType<CowBehavior>(
                FindObjectsSortMode.None
            );

        foreach (CowBehavior cow in cows)
        {
            // Measure the distance between the cow
            // and the potential spawn location.
            float distance = Vector2.Distance(
                spawnPoint.position,
                cow.transform.position
            );

            // Reject the spawn point if a cow is too close.
            if (distance < minimumCowDistance)
            {
                return true;
            }
        }

        // No nearby cows were found.
        return false;
    }
}