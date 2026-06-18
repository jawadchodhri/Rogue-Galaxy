using UnityEngine;
using UnityEngine.UI;
using System;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private int bossScoreValue = 100;
    public event Action OnBossKilled;
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Phase")]
    [SerializeField] private float phase2HealthPercent = 0.5f;

    [Header("UI")]
    [SerializeField] private Image healthFill;

    private float currentHealth;
    private bool isPhase2;

    public bool IsPhase2 => isPhase2;

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (!isPhase2 && currentHealth <= maxHealth * phase2HealthPercent)
        {
            isPhase2 = true;
            Debug.Log("Boss Phase 2 Started");
        }

        UpdateUI();

        if (currentHealth <= 0f)
            Die();
    }

    private void UpdateUI()
    {
        if (healthFill != null)
            healthFill.fillAmount = currentHealth / maxHealth;
    }

    private void Die()
    {
        if (GameStatsManager.Instance != null)
        {
            GameStatsManager.Instance.AddScore(bossScoreValue);
        }
        OnBossKilled?.Invoke();
        Debug.Log("Boss defeated");
        Destroy(gameObject);
    }
}