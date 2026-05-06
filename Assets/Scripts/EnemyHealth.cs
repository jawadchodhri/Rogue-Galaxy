using UnityEngine;
using UnityEngine.UI;


public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 3f;

    [Header("UI")]
    [SerializeField] private Image healthBar;

    private float currentHealth;
    private EnemyWaveMember waveMember;

    private void Awake()
    {
        waveMember = GetComponent<EnemyWaveMember>();
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHealthBar();

        if (currentHealth <= 0f)
        {
            if (waveMember != null)
                waveMember.DieByPlayer();
            else
                Destroy(gameObject);
        }
    }

    private void UpdateHealthBar()
        {
            if (healthBar == null) return;

            healthBar.fillAmount = currentHealth / maxHealth;
        }
}

    // [SerializeField] private float maxHealth = 1;

    // private float currentHealth;
    // private EnemyWaveMember waveMember;

    // private void Awake()
    // {
    //     waveMember = GetComponent<EnemyWaveMember>();
    // }

    // private void OnEnable()
    // {
    //     ResetHealth();
    // }

    // public void ResetHealth()
    // {
    //     currentHealth = maxHealth;
    // }

    // public void TakeDamage(float damage)
    // {
    //     currentHealth -= damage;

    //     if (currentHealth <= 0)
    //     {
    //         if (waveMember != null)
    //         {
    //             waveMember.DieByPlayer();
    //         }
    //         else
    //             Destroy(gameObject);
    //     }
    // }
