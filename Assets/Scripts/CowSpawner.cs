using System.Collections.Generic;
using UnityEngine;

public class CowSpawner : MonoBehaviour
{
    public GameObject cowPrefab;
    public Transform[] spawnPoints;

    public int targetCowCount = 7;
    public float spawnDelay = 2f;

    // How close another cow can be before this spawn point is rejected.
    public float minimumCowDistance = 2f;

    private float spawnTimer;

    // Remembers the most recently used spawn point.
    private Transform lastSpawnPoint;

    void Start()
    {
        spawnTimer = spawnDelay;
    }

    void Update()
    {
        int currentCowCount =
            FindObjectsByType<CowBehavior>(
                FindObjectsSortMode.None
            ).Length;

        if (currentCowCount >= targetCowCount)
        {
            spawnTimer = spawnDelay;
            return;
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnCow();
            spawnTimer = spawnDelay;
        }
    }

    void SpawnCow()
    {
        if (cowPrefab == null || spawnPoints.Length == 0)
            return;

        List<Transform> validSpawnPoints =
            new List<Transform>();

        foreach (Transform spawnPoint in spawnPoints)
        {
            // Don't spawn somewhere currently visible.
            if (IsSpawnPointVisible(spawnPoint))
                continue;

            // Don't immediately reuse the same spawn point.
            if (spawnPoint == lastSpawnPoint)
                continue;

            // Don't spawn too close to another cow.
            if (IsCowNearby(spawnPoint))
                continue;

            validSpawnPoints.Add(spawnPoint);
        }

        // If no ideal spawn points are available,
        // try again later instead of forcing a bad spawn.
        if (validSpawnPoints.Count == 0)
            return;

        Transform chosenSpawnPoint =
            validSpawnPoints[
                Random.Range(0, validSpawnPoints.Count)
            ];

        Instantiate(
            cowPrefab,
            chosenSpawnPoint.position,
            Quaternion.identity
        );

        lastSpawnPoint = chosenSpawnPoint;
    }

    bool IsSpawnPointVisible(Transform spawnPoint)
    {
        if (Camera.main == null)
            return false;

        Vector3 viewportPosition =
            Camera.main.WorldToViewportPoint(
                spawnPoint.position
            );

        return viewportPosition.x >= -0.1f &&
               viewportPosition.x <= 1.1f &&
               viewportPosition.y >= -0.1f &&
               viewportPosition.y <= 1.1f &&
               viewportPosition.z > 0f;
    }

    bool IsCowNearby(Transform spawnPoint)
    {
        CowBehavior[] cows =
            FindObjectsByType<CowBehavior>(
                FindObjectsSortMode.None
            );

        foreach (CowBehavior cow in cows)
        {
            float distance = Vector2.Distance(
                spawnPoint.position,
                cow.transform.position
            );

            if (distance < minimumCowDistance)
            {
                return true;
            }
        }

        return false;
    }
}