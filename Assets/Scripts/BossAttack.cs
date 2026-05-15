using UnityEngine;

public sealed class BossAttack : MonoBehaviour
{
    private enum AttackPattern
    {
        StraightShot,
        SpreadShot
    }

    [Header("References")]
    [SerializeField] private EnemyProjectile straightProjectilePrefab;
    [SerializeField] private EnemyProjectile spreadProjectilePrefab;
    [SerializeField] private Transform[] firePoints;

    [Header("Attack Timing")]
    [SerializeField] private float firstAttackDelay = 0.5f;
    [SerializeField] private float attackDelay = 1.2f;
    [SerializeField] private float phase2AttackDelay = 0.65f;

    [Header("Spread Shot")]
    [SerializeField] private float spreadAngle = 45f;
    [SerializeField] private float phase2SpreadAngle = 65f;

    private BossEntryMovement entryMovement;
    private BossHealth bossHealth;

    private float nextAttackTime;
    private bool attackTimerStarted;
    private int patternIndex;

    private readonly AttackPattern[] attackCycle =
    {
        AttackPattern.StraightShot,
        AttackPattern.SpreadShot
    };

    private void Awake()
    {
        entryMovement = GetComponent<BossEntryMovement>();
        bossHealth = GetComponent<BossHealth>();
    }

    private void Update()
    {
        if (CanAttack() == false)
            return;

        if (attackTimerStarted == false)
        {
            attackTimerStarted = true;
            nextAttackTime = Time.time + firstAttackDelay;
            return;
        }

        if (Time.time < nextAttackTime)
            return;

        StartNextAttack();
        SetNextAttackDelay();
    }

    private bool CanAttack()
    {
        if (straightProjectilePrefab == null)
            return false;

        if (spreadProjectilePrefab == null)
            return false;

        if (firePoints == null)
            return false;

        if (firePoints.Length == 0)
            return false;

        if (entryMovement != null)
        {
            if (entryMovement.HasEntered == false)
                return false;
        }

        return true;
    }

    private void StartNextAttack()
    {
        AttackPattern currentPattern = attackCycle[patternIndex];

        patternIndex++;

        if (patternIndex >= attackCycle.Length)
            patternIndex = 0;

        if (currentPattern == AttackPattern.StraightShot)
        {
            ShootStraight();
            return;
        }

        if (currentPattern == AttackPattern.SpreadShot)
        {
            ShootSpread();
        }
    }

    private void ShootStraight()
    {
        for (int i = 0; i < firePoints.Length; i++)
        {
            if (firePoints[i] == null)
                continue;

            SpawnProjectile(
                straightProjectilePrefab,
                firePoints[i].position,
                Vector2.down
            );
        }
    }

    private void ShootSpread()
    {
        float currentSpreadAngle = GetCurrentSpreadAngle();

        if (firePoints.Length == 1)
        {
            SpawnProjectile(
                spreadProjectilePrefab,
                firePoints[0].position,
                Vector2.down
            );

            return;
        }

        float startAngle = -currentSpreadAngle * 0.5f;
        float angleStep = currentSpreadAngle / (firePoints.Length - 1);

        for (int i = 0; i < firePoints.Length; i++)
        {
            if (firePoints[i] == null)
                continue;

            float angle = startAngle + angleStep * i;

            Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.down;
            direction.Normalize();

            SpawnProjectile(
                spreadProjectilePrefab,
                firePoints[i].position,
                direction
            );
        }
    }

    private float GetCurrentSpreadAngle()
    {
        if (IsPhase2() == true)
            return phase2SpreadAngle;

        return spreadAngle;
    }

    private void SpawnProjectile(EnemyProjectile prefab, Vector3 position, Vector2 direction)
    {
        EnemyProjectile projectile = Instantiate(
            prefab,
            position,
            Quaternion.identity
        );

        projectile.Initialize(direction);
    }

    private void SetNextAttackDelay()
    {
        float delay;

        if (IsPhase2() == true)
        {
            delay = phase2AttackDelay;
        }
        else
        {
            delay = attackDelay;
        }

        nextAttackTime = Time.time + delay;
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