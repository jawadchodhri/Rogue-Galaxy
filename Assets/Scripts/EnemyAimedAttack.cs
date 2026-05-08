using UnityEngine;

public class EnemyAimedAttack : MonoBehaviour
{
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackDelay = 2f;

    private Transform player;
    private float nextAttackTime;

    private void Awake()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    private void Update()
    {
        if (player == null) return;
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackDelay;
        Shoot();
    }

    private void Shoot()
    {
        Vector2 direction = player.position - firePoint.position;

        EnemyProjectile projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        projectile.Initialize(direction);
    }
}