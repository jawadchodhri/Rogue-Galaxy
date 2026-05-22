using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class Boss2DiveDashAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Boss2LaneMovement laneMovement;
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private Transform player;

    [Header("Optional Warning")]
    [SerializeField] private GameObject dashWarningPrefab;
    [SerializeField] private float warningY = 0f;

    [Header("Camera Clamp")]
    [SerializeField] private float horizontalPadding = 0.5f;

    [Header("Phase 1")]
    [SerializeField] private float lockOnDuration = 0.6f;
    [SerializeField] private float alignSpeed = 5f;
    [SerializeField] private float telegraphDuration = 0.45f;
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float returnSpeed = 7f;
    [SerializeField] private float dashEndY = -3.6f;

    [Header("Phase 2")]
    [SerializeField] private float phase2LockOnDuration = 0.45f;
    [SerializeField] private float phase2AlignSpeed = 7f;
    [SerializeField] private float phase2TelegraphDuration = 0.3f;
    [SerializeField] private float phase2DashSpeed = 16f;
    [SerializeField] private float phase2ReturnSpeed = 9f;
    [SerializeField] private int phase2DashCount = 2;

    public bool IsAttacking
    {
        get { return attackRoutine != null; }
    }

    public bool IsDashing
    {
        get { return isDashing; }
    }

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Coroutine attackRoutine;

    private float minX;
    private float maxX;

    private bool isDashing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        if (laneMovement == null)
        {
            laneMovement = GetComponent<Boss2LaneMovement>();
        }

        if (bossHealth == null)
        {
            bossHealth = GetComponent<BossHealth>();
        }

        rb.gravityScale = 0f;

        CalculateCameraBounds();
    }

    public void BeginAttack(Action onComplete)
    {
        if (attackRoutine != null)
            return;

        attackRoutine = StartCoroutine(DiveDashRoutine(onComplete));
    }

    private IEnumerator DiveDashRoutine(Action onComplete)
    {
        if (laneMovement != null)
        {
            laneMovement.PauseMovement();
        }

        int dashCount = GetDashCount();

        for (int i = 0; i < dashCount; i++)
        {
            yield return LockOnPlayerX();
            yield return Telegraph();
            yield return DashDown();
            yield return ReturnToTop();
        }

        if (laneMovement != null)
        {
            laneMovement.ResumeMovement();
        }

        attackRoutine = null;

        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }

    private IEnumerator LockOnPlayerX()
    {
        float duration = GetLockOnDuration();
        float speed = GetAlignSpeed();
        float endTime = Time.time + duration;

        while (Time.time < endTime)
        {
            float targetX = GetPlayerX();
            targetX = Mathf.Clamp(targetX, minX, maxX);

            Vector2 currentPosition = rb.position;

            Vector2 targetPosition = new Vector2(
                targetX,
                GetTopY()
            );

            Vector2 nextPosition = Vector2.MoveTowards(
                currentPosition,
                targetPosition,
                speed * Time.fixedDeltaTime
            );

            rb.MovePosition(nextPosition);

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator Telegraph()
    {
        GameObject warning = null;

        if (dashWarningPrefab != null)
        {
            warning = Instantiate(
                dashWarningPrefab,
                new Vector3(rb.position.x, warningY, 0f),
                Quaternion.identity
            );
        }

        float duration = GetTelegraphDuration();

        yield return new WaitForSeconds(duration);

        if (warning != null)
        {
            Destroy(warning);
        }
    }

    private IEnumerator DashDown()
    {
        isDashing = true;

        float speed = GetDashSpeed();

        Vector2 targetPosition = new Vector2(
            rb.position.x,
            dashEndY
        );

        while (Vector2.Distance(rb.position, targetPosition) > 0.03f)
        {
            Vector2 nextPosition = Vector2.MoveTowards(
                rb.position,
                targetPosition,
                speed * Time.fixedDeltaTime
            );

            rb.MovePosition(nextPosition);

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPosition);

        isDashing = false;
    }

    private IEnumerator ReturnToTop()
    {
        float speed = GetReturnSpeed();

        Vector2 targetPosition = new Vector2(
            rb.position.x,
            GetTopY()
        );

        while (Vector2.Distance(rb.position, targetPosition) > 0.03f)
        {
            Vector2 nextPosition = Vector2.MoveTowards(
                rb.position,
                targetPosition,
                speed * Time.fixedDeltaTime
            );

            rb.MovePosition(nextPosition);

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPosition);
    }

    private float GetPlayerX()
    {
        if (player == null)
            return rb.position.x;

        return player.position.x;
    }

    private float GetTopY()
    {
        if (laneMovement != null)
        {
            if (laneMovement.HasStartedMovement == true)
            {
                return laneMovement.HomeY;
            }
        }

        return rb.position.y;
    }

    private int GetDashCount()
    {
        if (IsPhase2() == true)
            return phase2DashCount;

        return 1;
    }

    private float GetLockOnDuration()
    {
        if (IsPhase2() == true)
            return phase2LockOnDuration;

        return lockOnDuration;
    }

    private float GetAlignSpeed()
    {
        if (IsPhase2() == true)
            return phase2AlignSpeed;

        return alignSpeed;
    }

    private float GetTelegraphDuration()
    {
        if (IsPhase2() == true)
            return phase2TelegraphDuration;

        return telegraphDuration;
    }

    private float GetDashSpeed()
    {
        if (IsPhase2() == true)
            return phase2DashSpeed;

        return dashSpeed;
    }

    private float GetReturnSpeed()
    {
        if (IsPhase2() == true)
            return phase2ReturnSpeed;

        return returnSpeed;
    }

    private void CalculateCameraBounds()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Boss2DiveDashAttack: Main Camera not found.");
            return;
        }

        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = halfHeight * mainCamera.aspect;

        minX = mainCamera.transform.position.x - halfWidth + horizontalPadding;
        maxX = mainCamera.transform.position.x + halfWidth - horizontalPadding;
    }

    private bool IsPhase2()
    {
        if (bossHealth == null)
            return false;

        if (bossHealth.IsPhase2 == true)
            return true;

        return false;
    }
}