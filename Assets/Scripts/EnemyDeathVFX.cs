using UnityEngine;

public sealed class EnemyDeathVFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Timing")]
    [SerializeField] private float lifeTime = 0.45f;

    private float disableTime;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void OnEnable()
    {
        disableTime = Time.time + lifeTime;

        if (animator != null)
        {
            animator.Play(0, 0, 0f);
        }
    }

    private void Update()
    {
        if (Time.time < disableTime)
            return;

        gameObject.SetActive(false);
    }

    public void Play(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }
}