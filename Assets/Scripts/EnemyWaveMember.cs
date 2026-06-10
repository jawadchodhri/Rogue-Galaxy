using UnityEngine;

public class EnemyWaveMember : MonoBehaviour
{
    [SerializeField] private int scoreValue = 10;
    private EnemyWaveSpawner spawner;

    public void Initialize(EnemyWaveSpawner owner)
    {
        spawner = owner;

        EnemyStraightMovement straightMovement = GetComponent<EnemyStraightMovement>();
            if (straightMovement != null)
                straightMovement.Initialize(owner);

        EnemyZigZagMovement zigZagMovement = GetComponent<EnemyZigZagMovement>();
            if (zigZagMovement != null)
                zigZagMovement.Initialize(owner);

        // EnemyStraightMovement straightMovement = GetComponent<EnemyStraightMovement>();
        //     if (straightMovement != null)
        //         straightMovement.Initialize(owner);
    }

    public void DieByPlayer()
    {

        if (GameStatsManager.Instance != null)
        {
            GameStatsManager.Instance.AddScore(scoreValue);
        }
        if (EnemyDeathVFXPool.Instance != null)
        {
            EnemyDeathVFXPool.Instance.Play(transform.position);
        }

        if (spawner != null)
            spawner.OnEnemyKilled();

        Destroy(gameObject);
    }
}
