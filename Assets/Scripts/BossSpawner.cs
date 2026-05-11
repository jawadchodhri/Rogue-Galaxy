using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform spawnPoint;

    private GameObject currentBoss;

    public void SpawnBoss()
    {
        if (bossPrefab == null || spawnPoint == null) return;
        if (currentBoss != null) return;

        currentBoss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
    }
}