using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float damage;

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
        Destroy(gameObject);
        return;
    }

    EnemyHealth enemy = other.GetComponent<EnemyHealth>();

    if (enemy != null)
    {
        hasHit = true;
        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}


    private void Disable()
    {
        Destroy(gameObject);
    }
}
