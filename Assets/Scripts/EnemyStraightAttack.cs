using UnityEngine;

public class EnemyStraightAttack : MonoBehaviour
{
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackDelay;

    private float nextAttackTime;

    private void Update()
    {
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackDelay;
        Shoot();
    }

    private void Shoot()
    {
        EnemyProjectile projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        projectile.Initialize(Vector2.down);
    }
}
