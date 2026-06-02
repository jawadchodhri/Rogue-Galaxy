using UnityEngine;

public sealed class CraneFlyPollenDropAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CraneFlyEnemyMovement movement;
    [SerializeField] private EnemyProjectile pollenProjectilePrefab;
    [SerializeField] private Transform[] firePoints;

    [Header("Attack Timing")]
    [SerializeField] private float firstAttackDelay = 0.6f;
    [SerializeField] private float minAttackDelay = 1.2f;
    [SerializeField] private float maxAttackDelay = 2f;

    private float nextAttackTime;
    private bool attackTimerStarted;

    private void Awake()
    {
        if (movement == null)
        {
            movement = GetComponent<CraneFlyEnemyMovement>();
        }
    }

    private void Update()
    {
        if (CanAttack() == false)
        {
            attackTimerStarted = false;
            return;
        }

        if (attackTimerStarted == false)
        {
            attackTimerStarted = true;
            nextAttackTime = Time.time + firstAttackDelay;
            return;
        }

        if (Time.time < nextAttackTime)
            return;

        ShootPollenDrop();
        SetNextAttackTime();
    }

    private bool CanAttack()
    {
        if (movement == null)
            return false;

        if (movement.CanAttack == false)
            return false;

        if (pollenProjectilePrefab == null)
            return false;

        if (firePoints == null)
            return false;

        if (firePoints.Length == 0)
            return false;

        return true;
    }

    private void ShootPollenDrop()
    {
        for (int i = 0; i < firePoints.Length; i++)
        {
            if (firePoints[i] == null)
                continue;

            EnemyProjectile projectile = Instantiate(
                pollenProjectilePrefab,
                firePoints[i].position,
                Quaternion.identity
            );

            projectile.Initialize(Vector2.down);
        }
    }

    private void SetNextAttackTime()
    {
        float delay = Random.Range(minAttackDelay, maxAttackDelay);
        nextAttackTime = Time.time + delay;
    }
}