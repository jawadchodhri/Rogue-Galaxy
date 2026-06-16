using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float collisionDamage = 20f;
    [SerializeField] private float damageCooldown = 0.5f;
    [SerializeField] private PlayerDamageImmunity damageImmunity;
    private float nextDamageTime;

    [Header("UI")]
    [SerializeField] private GameOverManager gameOverManager;
    [SerializeField] private Image healthFill;
    [SerializeField] private RespawnOfferManager respawnOfferManager;
    // [SerializeField] private PlayerDamageImmunity damageImmunity;

    private float currentHealth;

    private void Awake()
    {
        if (damageImmunity == null)
        {
            damageImmunity = GetComponent<PlayerDamageImmunity>();
        }

        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f)
        return;

        if (damageImmunity != null)
        {
            if (damageImmunity.IsImmune == true)
                return;
        }
        currentHealth -= damage;

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            Die();
        }

        if (damageImmunity != null)
        {
            damageImmunity.StartImmunity();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthFill == null) return;

        healthFill.fillAmount = currentHealth / maxHealth;
    }

    public void RespawnFromDeath()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (damageImmunity != null)
        {
            damageImmunity.StartImmunity();
        }
    }

    private void Die()
    {
        if (respawnOfferManager != null)
        {
            respawnOfferManager.ShowRespawnOffer(this);
            return;
        }

        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }

        //gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy == null)
            enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy == null) return;

        TakeDamage(collisionDamage);
        nextDamageTime = Time.time + damageCooldown;

        // EnemyWaveMember waveMember = other.GetComponent<EnemyWaveMember>();

        // if (waveMember == null)
        //     waveMember = other.GetComponentInParent<EnemyWaveMember>();

        // if (waveMember != null)
        //     waveMember.DieByPlayer();
        // else
        //     TakeDamage(collisionDamage);
    }

}