using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [SerializeField] private BossEntryMovement entryMovement;
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private float attackDelay = 1.2f;

    private float nextAttackTime;

    private void Update()
    {
        if (entryMovement != null && !entryMovement.HasEntered) return;
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackDelay;
        Shoot();
    }

    private void Shoot()
    {
        for (int i = 0; i < firePoints.Length; i++)
        {
            EnemyProjectile projectile = Instantiate(
                projectilePrefab,
                firePoints[i].position,
                Quaternion.identity
            );

            projectile.Initialize(Vector2.down);
        }
    }
}