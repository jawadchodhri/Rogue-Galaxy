using UnityEngine;

public sealed class MosquitoBloodAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MosquitoSideSlideMovement movement;
    [SerializeField] private EnemyProjectile bloodProjectilePrefab;
    [SerializeField] private Transform[] firePoints;

    [Header("Attack Timing")]
    [SerializeField] private float attackDuration = 0.8f;
    [SerializeField] private float projectileSpawnInterval = 0.08f;

    private bool isAttacking;
    private bool hasUsedCurrentAttackWindow;

    private float attackEndTime;
    private float nextProjectileTime;

    private void Awake()
    {
        if (movement == null)
        {
            movement = GetComponent<MosquitoSideSlideMovement>();
        }
    }

    private void Update()
    {
        if (CanStartAttack() == true)
        {
            StartAttack();
        }

        if (isAttacking == true)
        {
            HandleAttack();
        }

        if (movement != null)
        {
            if (movement.CanAttack == false)
            {
                ResetAttackWindow();
            }
        }
    }

    private bool CanStartAttack()
    {
        if (movement == null)
            return false;

        if (movement.CanAttack == false)
            return false;

        if (hasUsedCurrentAttackWindow == true)
            return false;

        if (bloodProjectilePrefab == null)
            return false;

        if (firePoints == null)
            return false;

        if (firePoints.Length == 0)
            return false;

        return true;
    }

    private void StartAttack()
    {
        isAttacking = true;
        hasUsedCurrentAttackWindow = true;

        attackEndTime = Time.time + attackDuration;
        nextProjectileTime = Time.time;

        FireBloodProjectiles();
    }

    private void HandleAttack()
    {
        if (Time.time >= attackEndTime)
        {
            StopAttack();
            return;
        }

        if (movement == null)
        {
            StopAttack();
            return;
        }

        if (movement.CanAttack == false)
        {
            StopAttack();
            return;
        }

        if (Time.time < nextProjectileTime)
            return;

        FireBloodProjectiles();

        nextProjectileTime = Time.time + projectileSpawnInterval;
    }

    private void FireBloodProjectiles()
    {
        for (int i = 0; i < firePoints.Length; i++)
        {
            if (firePoints[i] == null)
                continue;

            EnemyProjectile projectile = Instantiate(
                bloodProjectilePrefab,
                firePoints[i].position,
                Quaternion.identity
            );

            projectile.Initialize(Vector2.down);
        }
    }

    private void StopAttack()
    {
        isAttacking = false;
    }

    private void ResetAttackWindow()
    {
        isAttacking = false;
        hasUsedCurrentAttackWindow = false;
    }
}