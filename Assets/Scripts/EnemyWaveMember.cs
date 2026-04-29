using UnityEngine;

public class EnemyWaveMember : MonoBehaviour
{
    private EnemyWaveSpawner spawner;

    public void Initialize(EnemyWaveSpawner owner)
    {
        spawner = owner;
    }

    public void Die()
    {
        if (spawner != null)
            spawner.OnEnemyKilled();

        Destroy(gameObject);
    }
}
