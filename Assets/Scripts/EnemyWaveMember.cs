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
    }

    public void DieByPlayer()
    {
        if (spawner != null)
            spawner.OnEnemyKilled();

        gameObject.SetActive(false);
    }
}
