using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 4f;
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private float damage = 10f;

    private Vector2 moveDirection;

    public void Initialize(Vector2 direction)
    {
        moveDirection = direction.normalized;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player == null)
            player = other.GetComponentInParent<PlayerHealth>();

        if (player == null) return;

        player.TakeDamage(damage);
        Destroy(gameObject);
    }
}
