using System.Collections;
using UnityEngine;

public class SetFlowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyWaveSpawner waveSpawner;
    [SerializeField] private CoinPatternSpawner coinPatternSpawner;
    [SerializeField] private BossSpawner bossSpawner;

    [Header("Cool Off")]
    [SerializeField] private float coolOffDuration = 10f;

    private void OnEnable()
    {
        waveSpawner.OnAllWavesCompleted += StartCoolOffPeriod;
    }

    private void OnDisable()
    {
        waveSpawner.OnAllWavesCompleted -= StartCoolOffPeriod;
    }

    private void StartCoolOffPeriod()
    {
        StartCoroutine(CoolOffRoutine());
    }

    private IEnumerator CoolOffRoutine()
    {
        coinPatternSpawner.StartPatterns();

        yield return new WaitForSeconds(coolOffDuration);

        coinPatternSpawner.StopPatterns();

        while (coinPatternSpawner.HasActiveCoins)
        {
            yield return null;
        }

        bossSpawner.SpawnBoss();
    }
}