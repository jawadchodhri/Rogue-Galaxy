using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class CockroachTopStopShooter : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float stopY = 3.5f;

    [Header("Shooting")]
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private float firstShootDelay = 0.5f;
    [SerializeField] private float shootDelay = 1.2f;

    private Rigidbody2D rb;

    private bool hasStopped;
    private bool shootTimerStarted;

    private float nextShootTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (hasStopped == true)
            return;

        MoveDownUntilStopPoint();
    }

    private void Update()
    {
        if (hasStopped == false)
            return;

        HandleShooting();
    }

    private void MoveDownUntilStopPoint()
    {
        Vector2 currentPosition = rb.position;

        if (currentPosition.y <= stopY)
        {
            currentPosition.y = stopY;
            rb.MovePosition(currentPosition);

            hasStopped = true;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 nextPosition = currentPosition + Vector2.down * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);
    }

    private void HandleShooting()
    {
        if (projectilePrefab == null)
            return;

        if (firePoints == null || firePoints.Length == 0)
            return;

        if (shootTimerStarted == false)
        {
            shootTimerStarted = true;
            nextShootTime = Time.time + firstShootDelay;
            return;
        }

        if (Time.time < nextShootTime)
            return;

        Shoot();
        nextShootTime = Time.time + shootDelay;
    }

    private void Shoot()
    {
        for (int i = 0; i < firePoints.Length; i++)
        {
            if (firePoints[i] == null)
                continue;

            EnemyProjectile projectile = Instantiate(
                projectilePrefab,
                firePoints[i].position,
                Quaternion.identity
            );

            projectile.Initialize(Vector2.down);
        }
    }
}