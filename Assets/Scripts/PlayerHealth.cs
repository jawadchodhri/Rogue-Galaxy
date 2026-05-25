using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float collisionDamage = 20f;
    [SerializeField] private float damageCooldown = 0.5f;
    private float nextDamageTime;

    [Header("UI")]
    [SerializeField] private Image healthFill;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthFill == null) return;

        healthFill.fillAmount = currentHealth / maxHealth;
    }

    private void Die()
    {
        Debug.Log("Player Died");

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy == null)
            enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy == null) return;

        TakeDamage(collisionDamage);
        nextDamageTime = Time.time + damageCooldown;

        BossHealth bossHealth = other.GetComponent<BossHealth>();

        if (bossHealth != null)
        {
            TakeDamage(collisionDamage);
        }

        // EnemyWaveMember waveMember = other.GetComponent<EnemyWaveMember>();

        // if (waveMember == null)
        //     waveMember = other.GetComponentInParent<EnemyWaveMember>();

        // if (waveMember != null)
        //     waveMember.DieByPlayer();
        // else
        //     TakeDamage(collisionDamage);
    }

}