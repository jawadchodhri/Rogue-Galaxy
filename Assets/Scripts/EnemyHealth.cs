using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 1;

    private int currentHealth;
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

    public void TakeDamage(int damage)
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
