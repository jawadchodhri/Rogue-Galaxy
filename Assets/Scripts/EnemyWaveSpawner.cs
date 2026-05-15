using System;
using System.Collections;
using UnityEngine;

public class EnemyWaveSpawner : MonoBehaviour
{
    public event Action OnAllWavesCompleted;

    public enum BatchSpawnMode
    {
        Sequential,
        RandomMixed
    }

    [Serializable]
    public class EnemyBatchEnemy
    {
        public GameObject enemyPrefab;
        public int count = 1;

        [Header("Optional Specific Spawn")]
        public Transform specificSpawnPoint;
    }

    [Serializable]
    public class EnemyBatch
    {
        public string batchName;

        [Header("Enemies In This Batch")]
        public BatchSpawnMode spawnMode = BatchSpawnMode.Sequential;
        public EnemyBatchEnemy[] enemies;

        [Header("Timing")]
        public float spawnDelay = 0.5f;
    }

    [Serializable]
    public class EnemyWave
    {
        public string waveName;
        public EnemyBatch[] batches;
        public float delayBeforeWave = 1f;
    }

    [Header("Waves")]
    [SerializeField] private EnemyWave[] waves;

    [Header("Default Spawn Area")]
    [SerializeField] private float spawnY = 6f;
    [SerializeField] private float horizontalPadding = 0.5f;

    [Header("Timing")]
    [SerializeField] private float delayBetweenBatches = 1f;

    private Camera mainCamera;

    private float minX;
    private float maxX;

    private int currentWaveIndex;
    private int currentBatchIndex;
    private int aliveEnemies;

    private bool isSpawning;
    private bool waitingForNextBatch;

    private int[] randomRemainingCounts;

    private void Awake()
    {
        mainCamera = Camera.main;
        CalculateCameraBounds();
    }

    private void Start()
    {
        StartCoroutine(StartWaveRoutine());
    }

    private IEnumerator StartWaveRoutine()
    {
        if (waves == null)
            yield break;

        if (waves.Length == 0)
            yield break;

        if (currentWaveIndex >= waves.Length)
            yield break;

        EnemyWave wave = waves[currentWaveIndex];

        yield return new WaitForSeconds(wave.delayBeforeWave);

        currentBatchIndex = 0;

        StartCoroutine(SpawnBatchRoutine());
    }

    private IEnumerator SpawnBatchRoutine()
    {
        if (currentWaveIndex >= waves.Length)
            yield break;

        EnemyWave wave = waves[currentWaveIndex];

        if (wave.batches == null || wave.batches.Length == 0)
        {
            StartNextWave();
            yield break;
        }

        if (currentBatchIndex >= wave.batches.Length)
        {
            StartNextWave();
            yield break;
        }

        EnemyBatch batch = wave.batches[currentBatchIndex];

        isSpawning = true;
        waitingForNextBatch = false;
        aliveEnemies = 0;

        if (batch.enemies == null || batch.enemies.Length == 0)
        {
            isSpawning = false;
            StartNextBatchSafely();
            yield break;
        }

        if (batch.spawnMode == BatchSpawnMode.Sequential)
        {
            yield return SpawnSequentialBatch(batch);
        }

        if (batch.spawnMode == BatchSpawnMode.RandomMixed)
        {
            yield return SpawnRandomMixedBatch(batch);
        }

        isSpawning = false;

        if (aliveEnemies <= 0)
        {
            StartNextBatchSafely();
        }
    }

    private IEnumerator SpawnSequentialBatch(EnemyBatch batch)
    {
        for (int i = 0; i < batch.enemies.Length; i++)
        {
            EnemyBatchEnemy enemyData = batch.enemies[i];

            if (enemyData == null)
                continue;

            if (enemyData.enemyPrefab == null)
                continue;

            if (enemyData.count <= 0)
                continue;

            for (int j = 0; j < enemyData.count; j++)
            {
                bool spawned = SpawnEnemy(enemyData);

                if (spawned == true)
                    aliveEnemies++;

                yield return new WaitForSeconds(batch.spawnDelay);
            }
        }
    }

    private IEnumerator SpawnRandomMixedBatch(EnemyBatch batch)
    {
        int totalEnemyCount = PrepareRandomBatchCounts(batch);

        while (totalEnemyCount > 0)
        {
            int selectedIndex = GetRandomEnemyIndex(batch, totalEnemyCount);

            if (selectedIndex < 0)
                yield break;

            EnemyBatchEnemy enemyData = batch.enemies[selectedIndex];

            bool spawned = SpawnEnemy(enemyData);

            if (spawned == true)
                aliveEnemies++;

            randomRemainingCounts[selectedIndex]--;
            totalEnemyCount--;

            yield return new WaitForSeconds(batch.spawnDelay);
        }
    }

    private int PrepareRandomBatchCounts(EnemyBatch batch)
    {
        if (randomRemainingCounts == null)
        {
            randomRemainingCounts = new int[batch.enemies.Length];
        }

        if (randomRemainingCounts.Length < batch.enemies.Length)
        {
            randomRemainingCounts = new int[batch.enemies.Length];
        }

        int totalEnemyCount = 0;

        for (int i = 0; i < batch.enemies.Length; i++)
        {
            randomRemainingCounts[i] = 0;

            EnemyBatchEnemy enemyData = batch.enemies[i];

            if (enemyData == null)
                continue;

            if (enemyData.enemyPrefab == null)
                continue;

            if (enemyData.count <= 0)
                continue;

            randomRemainingCounts[i] = enemyData.count;
            totalEnemyCount += enemyData.count;
        }

        return totalEnemyCount;
    }

    private int GetRandomEnemyIndex(EnemyBatch batch, int totalEnemyCount)
    {
        if (totalEnemyCount <= 0)
            return -1;

        int randomValue = UnityEngine.Random.Range(0, totalEnemyCount);
        int currentValue = 0;

        for (int i = 0; i < batch.enemies.Length; i++)
        {
            currentValue += randomRemainingCounts[i];

            if (randomValue < currentValue)
                return i;
        }

        return -1;
    }

    private bool SpawnEnemy(EnemyBatchEnemy enemyData)
    {
        if (enemyData == null)
            return false;

        if (enemyData.enemyPrefab == null)
            return false;

        GameObject enemy = Instantiate(
            enemyData.enemyPrefab,
            GetSpawnPosition(enemyData),
            Quaternion.identity
        );

        EnemyWaveMember member = enemy.GetComponent<EnemyWaveMember>();

        if (member != null)
        {
            member.Initialize(this);
        }

        return true;
    }

    private Vector3 GetSpawnPosition(EnemyBatchEnemy enemyData)
    {
        if (enemyData.specificSpawnPoint != null)
        {
            return enemyData.specificSpawnPoint.position;
        }

        return GetRandomSpawnPosition();
    }

    public void OnEnemyKilled()
    {
        aliveEnemies--;

        if (aliveEnemies <= 0)
        {
            if (isSpawning == false)
            {
                StartNextBatchSafely();
            }
        }
    }

    private void StartNextBatchSafely()
    {
        if (waitingForNextBatch == true)
            return;

        waitingForNextBatch = true;
        StartCoroutine(StartNextBatchRoutine());
    }

    private IEnumerator StartNextBatchRoutine()
    {
        yield return new WaitForSeconds(delayBetweenBatches);

        waitingForNextBatch = false;

        currentBatchIndex++;

        if (currentWaveIndex >= waves.Length)
            yield break;

        EnemyWave wave = waves[currentWaveIndex];

        if (currentBatchIndex >= wave.batches.Length)
        {
            StartNextWave();
            yield break;
        }

        StartCoroutine(SpawnBatchRoutine());
    }

    private void StartNextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("All waves completed.");
            OnAllWavesCompleted?.Invoke();
            return;
        }

        StartCoroutine(StartWaveRoutine());
    }

    private void CalculateCameraBounds()
    {
        if (mainCamera == null)
        {
            Debug.LogError("EnemyWaveSpawner: Main Camera not found.");
            return;
        }

        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = halfHeight * mainCamera.aspect;

        minX = mainCamera.transform.position.x - halfWidth + horizontalPadding;
        maxX = mainCamera.transform.position.x + halfWidth - horizontalPadding;
    }

    public Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(
            UnityEngine.Random.Range(minX, maxX),
            spawnY,
            0f
        );
    }
}