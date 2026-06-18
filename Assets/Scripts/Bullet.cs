using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float damage;
    [Header("Score")]
    [SerializeField] private int enemyHitScore = 5;
    [SerializeField] private int bossHitScore = 10;

    private bool hasHit;

    private void OnEnable()
    {
        hasHit = false;
        Invoke(nameof(Disable), lifeTime);
    }


    private void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
{
    if (hasHit) return;

    BossHealth boss = other.GetComponent<BossHealth>();

    if (boss != null)
    {
        hasHit = true;
        boss.TakeDamage(damage);

        if (GameStatsManager.Instance != null)
        {
            GameStatsManager.Instance.AddScore(bossHitScore);
        }

        Vector3 hitPosition = other.ClosestPoint(transform.position);

        if (HitImpactVFXPool.Instance != null)
        {
            HitImpactVFXPool.Instance.Play(hitPosition, boss.transform);
        }

        Destroy(gameObject);
        return;
    }

    EnemyHealth enemy = other.GetComponent<EnemyHealth>();

    if (enemy != null)
    {
        hasHit = true;
        enemy.TakeDamage(damage);
        if (GameStatsManager.Instance != null)
        {
            GameStatsManager.Instance.AddScore(enemyHitScore);
        }

        Vector3 hitPosition = other.ClosestPoint(transform.position);

        if (HitImpactVFXPool.Instance != null)
        {
            HitImpactVFXPool.Instance.Play(hitPosition, enemy.transform);
        }

        Destroy(gameObject);
    }
}


    private void Disable()
    {
        Destroy(gameObject);
    }
}
