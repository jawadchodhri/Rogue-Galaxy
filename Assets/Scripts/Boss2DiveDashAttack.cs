using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class Boss2DiveDashAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Boss2LaneMovement laneMovement;
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private Collider2D bossCollider;

    [Header("Player Find")]
    [SerializeField] private string playerTag = "Player";

    [Header("Optional Warning")]
    [SerializeField] private GameObject dashWarningPrefab;
    [SerializeField] private float warningY = 0f;

    [Header("Camera Clamp")]
    [SerializeField] private float horizontalPadding = 0.35f;

    [Header("Phase 1")]
    [SerializeField] private float lockOnDuration = 0.6f;
    [SerializeField] private float alignSpeed = 8f;
    [SerializeField] private float telegraphDuration = 0.45f;
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float returnSpeed = 7f;
    [SerializeField] private float dashEndY = -3.6f;

    [Header("Phase 2")]
    [SerializeField] private float phase2LockOnDuration = 0.45f;
    [SerializeField] private float phase2AlignSpeed = 10f;
    [SerializeField] private float phase2TelegraphDuration = 0.3f;
    [SerializeField] private float phase2DashSpeed = 16f;
    [SerializeField] private float phase2ReturnSpeed = 9f;
    [SerializeField] private int phase2DashCount = 2;

    private PlayerHealth playerHealth;

    public bool IsAttacking
    {
        get { return attackRoutine != null; }
    }

    public bool IsDashing
    {
        get { return isDashing; }
    }

    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private Camera mainCamera;
    private Coroutine attackRoutine;

    private float minX;
    private float maxX;
    private float lockedDashX;

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

        if (bossCollider == null)
        {
            bossCollider = GetComponent<Collider2D>();
        }

        rb.gravityScale = 0f;

        CalculateCameraBounds();
        FindPlayer();
    }

    public void BeginAttack(Action onComplete)
    {
        if (attackRoutine != null)
            return;

        FindPlayer();

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
            float livePlayerX = GetLivePlayerX();
            livePlayerX = GetClampedX(livePlayerX);

            Vector2 targetPosition = new Vector2(
                livePlayerX,
                GetTopY()
            );

            Vector2 nextPosition = Vector2.MoveTowards(
                rb.position,
                targetPosition,
                speed * Time.fixedDeltaTime
            );

            rb.MovePosition(nextPosition);

            yield return new WaitForFixedUpdate();
        }

        lockedDashX = GetClampedX(GetLivePlayerX());

        Vector2 finalLockPosition = new Vector2(
            lockedDashX,
            GetTopY()
        );

        rb.MovePosition(finalLockPosition);
    }

    private IEnumerator Telegraph()
{
    GameObject warning = null;

    if (dashWarningPrefab != null)
    {
        warning = Instantiate(
            dashWarningPrefab,
            new Vector3(lockedDashX, warningY, 0f),
            Quaternion.identity
        );
    }

    yield return new WaitForSeconds(GetTelegraphDuration());

    if (warning != null)
    {
        Destroy(warning);
    }
}

    private IEnumerator DashDown()
    {
        isDashing = true;

        Vector2 targetPosition = new Vector2(
            lockedDashX,
            dashEndY
        );

        float speed = GetDashSpeed();

        while (Vector2.Distance(rb.position, targetPosition) > 0.03f)
        {
            Vector2 nextPosition = Vector2.MoveTowards(
                rb.position,
                targetPosition,
                speed * Time.fixedDeltaTime
            );

            nextPosition.x = lockedDashX;

            rb.MovePosition(nextPosition);

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPosition);
        isDashing = false;
    }

    private IEnumerator ReturnToTop()
    {
        Vector2 targetPosition = new Vector2(
            lockedDashX,
            GetTopY()
        );

        float speed = GetReturnSpeed();

        while (Vector2.Distance(rb.position, targetPosition) > 0.03f)
        {
            Vector2 nextPosition = Vector2.MoveTowards(
                rb.position,
                targetPosition,
                speed * Time.fixedDeltaTime
            );

            nextPosition.x = lockedDashX;

            rb.MovePosition(nextPosition);

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPosition);
    }

    private void FindPlayer()
    {
        if (playerHealth == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

            if (playerObject != null)
            {
                playerHealth = playerObject.GetComponent<PlayerHealth>();
            }
        }

        if (playerHealth != null)
        {
            playerRb = playerHealth.GetComponent<Rigidbody2D>();
        }

        if (playerHealth == null)
        {
            Debug.LogError("Boss2DiveDashAttack: PlayerHealth not found. Make sure Player has Tag = Player and PlayerHealth on root.");
        }
    }

    private float GetLivePlayerX()
    {
        if (playerRb != null)
        {
            return playerRb.position.x;
        }

        if (playerHealth != null)
        {
            return playerHealth.transform.position.x;
        }

        return rb.position.x;
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

    private float GetClampedX(float xPosition)
    {
        return Mathf.Clamp(xPosition, minX, maxX);
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

        float bossHalfWidth = 0f;

        if (bossCollider != null)
        {
            bossHalfWidth = bossCollider.bounds.extents.x;
        }

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