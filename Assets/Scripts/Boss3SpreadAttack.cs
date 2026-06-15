using UnityEngine;

public sealed class Boss3SpreadAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform[] firePoints;

    [Header("Spread Attack")]
    [SerializeField] private int spreadBulletCount = 5;
    [SerializeField] private int phase2SpreadBulletCount = 7;
    [SerializeField] private float spreadAngle = 65f;
    [SerializeField] private float phase2SpreadAngle = 85f;

    private void Awake()
    {
        if (bossHealth == null)
        {
            bossHealth = GetComponent<BossHealth>();
        }
    }

    public void ShootSpread()
    {
        if (projectilePrefab == null)
            return;

        if (firePoints == null)
            return;

        if (firePoints.Length == 0)
            return;

        int bulletCount = GetCurrentBulletCount();
        float currentSpreadAngle = GetCurrentSpreadAngle();

        if (bulletCount <= 1)
        {
            ShootFromAllFirePoints(Vector2.down);
            return;
        }

        float startAngle = -currentSpreadAngle * 0.5f;
        float angleStep = currentSpreadAngle / (bulletCount - 1);

        for (int i = 0; i < firePoints.Length; i++)
        {
            if (firePoints[i] == null)
                continue;

            for (int j = 0; j < bulletCount; j++)
            {
                float angle = startAngle + angleStep * j;

                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.down;
                direction.Normalize();

                SpawnProjectile(firePoints[i].position, direction);
            }
        }
    }

    private void ShootFromAllFirePoints(Vector2 direction)
    {
        for (int i = 0; i < firePoints.Length; i++)
        {
            if (firePoints[i] == null)
                continue;

            SpawnProjectile(firePoints[i].position, direction);
        }
    }

    private void SpawnProjectile(Vector3 position, Vector2 direction)
    {
        EnemyProjectile projectile = Instantiate(
            projectilePrefab,
            position,
            Quaternion.identity
        );

        projectile.Initialize(direction);
    }

    private int GetCurrentBulletCount()
    {
        if (IsPhase2() == true)
            return phase2SpreadBulletCount;

        return spreadBulletCount;
    }

    private float GetCurrentSpreadAngle()
    {
        if (IsPhase2() == true)
            return phase2SpreadAngle;

        return spreadAngle;
    }

    private bool IsPhase2()
    {
        if (bossHealth == null)
            return false;

        if (bossHealth.IsPhase2 == true)
            return true;

        return false;
    }
}