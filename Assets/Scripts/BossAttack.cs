using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform[] firePoints;

    [Header("Phase 1 Attack")]
    [SerializeField] private float attackDelay = 1.2f;

    [Header("Phase 2 Attack")]
    [SerializeField] private float phase2AttackDelay = 0.65f;

    private BossEntryMovement entryMovement;
    private BossHealth bossHealth;
    private float nextAttackTime;

    private void Awake()
    {
        entryMovement = GetComponent<BossEntryMovement>();
        bossHealth = GetComponent<BossHealth>();
    }

    private void Update()
    {
        NewAttack();
        Shoot();
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoints == null) return;

        for (int i = 0; i < firePoints.Length; i++)
        {
            if (firePoints[i] == null) continue;

            EnemyProjectile projectile = Instantiate(
                projectilePrefab,
                firePoints[i].position,
                Quaternion.identity
            );

            projectile.Initialize(Vector2.down);
        }
    }

    private void NewAttack()
    {
        if (entryMovement != null && !entryMovement.HasEntered)
            return;

        float currentAttackDelay =
            bossHealth != null && bossHealth.IsPhase2
                ? phase2AttackDelay
                : attackDelay;

        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + currentAttackDelay;
    }
}