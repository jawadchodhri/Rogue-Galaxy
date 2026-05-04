using System.Collections;
using UnityEngine;

public class EnemyWaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyBatch
    {
        public string batchName;
        public GameObject enemyPrefab;
        public int count = 3;
        public float spawnDelay = 0.5f;
    }

    [System.Serializable]
    public class EnemyWave
    {
        public string waveName;
        public EnemyBatch[] batches;
        public float delayBeforeWave = 1f;
    }

    [Header("Waves")]
    [SerializeField] private EnemyWave[] waves;

    [Header("Spawn Area")]
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
        if (waves == null || waves.Length == 0)
            yield break;

        EnemyWave wave = waves[currentWaveIndex];

        yield return new WaitForSeconds(wave.delayBeforeWave);

        currentBatchIndex = 0;

        StartCoroutine(SpawnBatchRoutine());
    }

    private IEnumerator SpawnBatchRoutine()
    {
        EnemyWave wave = waves[currentWaveIndex];

        if (wave.batches == null || wave.batches.Length == 0)
            yield break;

        if (currentBatchIndex >= wave.batches.Length)
        {
            StartNextWave();
            yield break;
        }

        EnemyBatch batch = wave.batches[currentBatchIndex];

        isSpawning = true;
        aliveEnemies = 0;

        for (int i = 0; i < batch.count; i++)
        {
            SpawnEnemy(batch.enemyPrefab);
            aliveEnemies++;

            yield return new WaitForSeconds(batch.spawnDelay);
        }

        isSpawning = false;
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null) return;

        GameObject enemy = Instantiate(
            enemyPrefab,
            GetRandomSpawnPosition(),
            Quaternion.identity
        );

        EnemyWaveMember member = enemy.GetComponent<EnemyWaveMember>();

        if (member != null)
            member.Initialize(this);
    }

    public void OnEnemyKilled()
    {
        aliveEnemies--;

        if (aliveEnemies <= 0 && !isSpawning)
        {
            StartCoroutine(StartNextBatchRoutine());
        }
    }

    private IEnumerator StartNextBatchRoutine()
    {
        yield return new WaitForSeconds(delayBetweenBatches);

        currentBatchIndex++;

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
            return;
        }

        StartCoroutine(StartWaveRoutine());
    }

    private void CalculateCameraBounds()
    {
        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = halfHeight * mainCamera.aspect;

        minX = mainCamera.transform.position.x - halfWidth + horizontalPadding;
        maxX = mainCamera.transform.position.x + halfWidth - horizontalPadding;
    }

    public Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(
            Random.Range(minX, maxX),
            spawnY,
            0f
        );
    }
}
