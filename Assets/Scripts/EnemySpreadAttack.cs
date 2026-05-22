using UnityEngine;

public class EnemySpreadAttack : MonoBehaviour
{
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackDelay = 2.5f;
    [SerializeField] private float spreadAngle = 20f;

    private EnemyVisibilityGate visibilityGate;
    private float nextAttackTime;

    private void Awake()
    {
        visibilityGate = GetComponent<EnemyVisibilityGate>();
    }

    private void Update()
    {
        if (visibilityGate != null && !visibilityGate.HasEnteredCamera)
            return;

        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackDelay;
        ShootSpread();
    }

    private void ShootSpread()
    {
        Shoot(Vector2.down);

        Vector2 leftDirection = Quaternion.Euler(0f, 0f, spreadAngle) * Vector2.down;
        Vector2 rightDirection = Quaternion.Euler(0f, 0f, -spreadAngle) * Vector2.down;

        Shoot(leftDirection);
        Shoot(rightDirection);
    }

    private void Shoot(Vector2 direction)
    {
        EnemyProjectile projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        projectile.Initialize(direction);
    }
}