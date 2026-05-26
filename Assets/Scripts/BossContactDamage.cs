using UnityEngine;

public sealed class BossContactDamage : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float damageCooldown;

    private float nextDamageTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }


    private void TryDamage(Collider2D other)
    {
        if (Time.time < nextDamageTime)
            return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            return;

        playerHealth.TakeDamage(damage);
        nextDamageTime = Time.time + damageCooldown;

        Debug.Log("Boss damaged player.");
    }
}