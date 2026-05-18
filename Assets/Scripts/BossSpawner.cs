using UnityEngine;
using System;

public class BossSpawner : MonoBehaviour
{
    public event Action OnBossKilled;

    [Header("Default Spawn")]
    [SerializeField] private Transform defaultSpawnPoint;

    private BossHealth currentBoss;

    public void SpawnBoss(BossHealth bossPrefab, Transform spawnPoint)
    {
        if (bossPrefab == null)
            return;

        Vector3 spawnPosition = transform.position;

        if (defaultSpawnPoint != null)
        {
            spawnPosition = defaultSpawnPoint.position;
        }

        if (spawnPoint != null)
        {
            spawnPosition = spawnPoint.position;
        }

        currentBoss = Instantiate(
            bossPrefab,
            spawnPosition,
            Quaternion.identity
        );

        currentBoss.OnBossKilled += HandleBossKilled;
    }

    private void HandleBossKilled()
    {
        if (currentBoss != null)
        {
            currentBoss.OnBossKilled -= HandleBossKilled;
        }

        currentBoss = null;

        OnBossKilled?.Invoke();
    }
    // [SerializeField] private GameObject bossPrefab;
    // [SerializeField] private Transform spawnPoint;

    // private GameObject currentBoss;

    // public void SpawnBoss()
    // {
    //     if (bossPrefab == null || spawnPoint == null) return;
    //     if (currentBoss != null) return;

    //     currentBoss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
    // }
}