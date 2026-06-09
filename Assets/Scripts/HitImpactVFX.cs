using UnityEngine;

public sealed class HitImpactVFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Timing")]
    [SerializeField] private float lifeTime = 0.25f;

    private Transform followTarget;
    private Vector3 localOffset;

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
        FollowTarget();

        if (Time.time < disableTime)
            return;

        Disable();
    }

    public void Play(Vector3 worldPosition, Transform target)
    {
        followTarget = target;

        if (followTarget != null)
        {
            localOffset = followTarget.InverseTransformPoint(worldPosition);
        }
        else
        {
            localOffset = Vector3.zero;
        }

        transform.position = worldPosition;
        gameObject.SetActive(true);
    }

    private void FollowTarget()
    {
        if (followTarget == null)
            return;

        transform.position = followTarget.TransformPoint(localOffset);
    }

    private void Disable()
    {
        followTarget = null;
        gameObject.SetActive(false);
    }
}