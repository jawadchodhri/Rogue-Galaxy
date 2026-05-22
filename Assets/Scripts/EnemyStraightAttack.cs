using UnityEngine;

public class EnemyStraightAttack : MonoBehaviour
{
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackDelay;

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
