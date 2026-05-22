using UnityEngine;

public sealed class BossContactDamage : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float damageCooldown = 0.5f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private float nextDamageTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider2D other)
    {
        if (Time.time < nextDamageTime)
            return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            if (debugLogs == true)
            {
                Debug.Log("Boss touched: " + other.name + " but no PlayerHealth found.");
            }

            return;
        }

        playerHealth.TakeDamage(damage);

        if (debugLogs == true)
        {
            Debug.Log("Boss damaged player: " + damage);
        }

        nextDamageTime = Time.time + damageCooldown;
    }
}