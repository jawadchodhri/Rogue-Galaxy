using System.Collections;
using UnityEngine;
using System;

public class SetFlowController : MonoBehaviour
{
    public event Action OnAllSetsCompleted;

    [Serializable]
    public class GameSet
    {
        public string setName;

        [Header("Enemy Waves")]
        public EnemyWaveSpawner.EnemyWave[] waves;

        [Header("Coin Phase")]
        public float coolOffDuration = 10f;

        [Header("Boss")]
        public BossHealth bossPrefab;
        public Transform bossSpawnPoint;

        [Header("Set Timing")]
        public float delayAfterBossDeath = 2f;
    }

    [Header("References")]
    [SerializeField] private EnemyWaveSpawner waveSpawner;
    [SerializeField] private CoinPatternSpawner coinPatternSpawner;
    [SerializeField] private BossSpawner bossSpawner;

    [Header("Sets")]
    [SerializeField] private GameSet[] sets;

    private int currentSetIndex;
    private bool flowRunning;

    private void OnEnable()
    {
        if (waveSpawner != null)
        {
            waveSpawner.OnAllWavesCompleted += HandleWavesCompleted;
        }

        if (bossSpawner != null)
        {
            bossSpawner.OnBossKilled += HandleBossKilled;
        }
    }

    private void OnDisable()
    {
        if (waveSpawner != null)
        {
            waveSpawner.OnAllWavesCompleted -= HandleWavesCompleted;
        }

        if (bossSpawner != null)
        {
            bossSpawner.OnBossKilled -= HandleBossKilled;
        }
    }

    private void Start()
    {
        StartCurrentSet();
    }

    private void StartCurrentSet()
    {
        if (flowRunning == true)
            return;

        if (sets == null)
            return;

        if (sets.Length == 0)
            return;

        if (currentSetIndex >= sets.Length)
        {
            CompleteAllSets();
            return;
        }

        GameSet currentSet = sets[currentSetIndex];

        if (currentSet == null)
            return;

        Debug.Log("Starting Set: " + currentSet.setName);

        flowRunning = true;

        if (waveSpawner != null)
        {
            waveSpawner.StartWaves(currentSet.waves);
        }
    }

    private void HandleWavesCompleted()
    {
        StartCoroutine(CoinPhaseRoutine());
    }

    private IEnumerator CoinPhaseRoutine()
    {
        GameSet currentSet = sets[currentSetIndex];

        if (coinPatternSpawner != null)
        {
            coinPatternSpawner.StartPatterns();
        }

        yield return new WaitForSeconds(currentSet.coolOffDuration);

        if (coinPatternSpawner != null)
        {
            coinPatternSpawner.StopPatterns();

            while (coinPatternSpawner.HasActiveCoins == true)
            {
                yield return null;
            }
        }

        SpawnCurrentSetBoss();
    }

    private void SpawnCurrentSetBoss()
    {
        GameSet currentSet = sets[currentSetIndex];

        if (bossSpawner == null)
            return;

        if (currentSet.bossPrefab == null)
            return;

        bossSpawner.SpawnBoss(
            currentSet.bossPrefab,
            currentSet.bossSpawnPoint
        );
    }

    private void HandleBossKilled()
    {
        StartCoroutine(NextSetRoutine());
    }

    private IEnumerator NextSetRoutine()
    {
        GameSet currentSet = sets[currentSetIndex];

        yield return new WaitForSeconds(currentSet.delayAfterBossDeath);

        currentSetIndex++;
        flowRunning = false;

        if (currentSetIndex >= sets.Length)
        {
            CompleteAllSets();
            yield break;
        }

        StartCurrentSet();
    }

    private void CompleteAllSets()
    {
        Debug.Log("All sets completed.");
        OnAllSetsCompleted?.Invoke();
    }
    // [Header("References")]
    // [SerializeField] private EnemyWaveSpawner waveSpawner;
    // [SerializeField] private CoinPatternSpawner coinPatternSpawner;
    // [SerializeField] private BossSpawner bossSpawner;

    // [Header("Cool Off")]
    // [SerializeField] private float coolOffDuration = 10f;

    // private void OnEnable()
    // {
    //     waveSpawner.OnAllWavesCompleted += StartCoolOffPeriod;
    // }

    // private void OnDisable()
    // {
    //     waveSpawner.OnAllWavesCompleted -= StartCoolOffPeriod;
    // }

    // private void StartCoolOffPeriod()
    // {
    //     StartCoroutine(CoolOffRoutine());
    // }

    // private IEnumerator CoolOffRoutine()
    // {
    //     coinPatternSpawner.StartPatterns();

    //     yield return new WaitForSeconds(coolOffDuration);

    //     coinPatternSpawner.StopPatterns();

    //     while (coinPatternSpawner.HasActiveCoins)
    //     {
    //         yield return null;
    //     }

    //     bossSpawner.SpawnBoss();
    // }
}