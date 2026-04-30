using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 1;

    private float currentHealth;
    private EnemyWaveMember waveMember;

    private void Awake()
    {
        waveMember = GetComponent<EnemyWaveMember>();
    }

    private void OnEnable()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            if (waveMember != null)
            {
                waveMember.Die();
            }
            else
                Destroy(gameObject);
        }
    }
}
