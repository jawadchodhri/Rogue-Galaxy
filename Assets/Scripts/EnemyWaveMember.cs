using UnityEngine;

public class EnemyWaveMember : MonoBehaviour
{
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
        if (EnemyDeathVFXPool.Instance != null)
        {
            EnemyDeathVFXPool.Instance.Play(transform.position);
        }

        if (spawner != null)
            spawner.OnEnemyKilled();

        Destroy(gameObject);
    }
}
