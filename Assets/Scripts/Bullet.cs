using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 1;

    private void OnEnable()
    {
        // Auto destroy to avoid leaks
        Invoke(nameof(Disable), lifeTime);
    }

    private void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy == null) return;

        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }

    private void Disable()
    {
        Destroy(gameObject);
    }
}
