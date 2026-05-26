using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class Boss2LaneMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossEntryMovement entryMovement;
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private Collider2D bossCollider;

    [Header("Lanes")]
    [SerializeField] private float[] laneXPositions = { -1.8f, 0f, 1.8f };

    [Header("Camera Clamp")]
    [SerializeField] private bool clampInsideCamera = true;
    [SerializeField] private float horizontalPadding = 0.35f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float phase2MoveSpeed = 3.8f;

    [Header("Wait Between Lane Moves")]
    [SerializeField] private float minWaitTime = 0.7f;
    [SerializeField] private float maxWaitTime = 1.2f;
    [SerializeField] private float phase2MinWaitTime = 0.35f;
    [SerializeField] private float phase2MaxWaitTime = 0.7f;

    public float HomeY
    {
        get { return homeY; }
    }

    public bool HasStartedMovement
    {
        get { return hasStartedMovement; }
    }

    public bool IsPaused
    {
        get { return isPaused; }
    }

    private Rigidbody2D rb;
    private Camera mainCamera;

    private float homeY;
    private float waitTimer;
    private float targetX;

    private float minX;
    private float maxX;

    private int currentLaneIndex;

    private bool hasStartedMovement;
    private bool isPaused;
    private bool isWaiting;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        if (entryMovement == null)
        {
            entryMovement = GetComponent<BossEntryMovement>();
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
    }

    private void FixedUpdate()
    {
        if (CanMove() == false)
            return;

        if (hasStartedMovement == false)
        {
            StartLaneMovement();
            return;
        }

        if (isPaused == true)
            return;

        if (isWaiting == true)
        {
            HandleWait();
            return;
        }

        MoveToLane();
    }

    public void PauseMovement()
    {
        isPaused = true;
    }

    public void ResumeMovement()
    {
        isPaused = false;
    }

    private bool CanMove()
    {
        if (entryMovement != null)
        {
            if (entryMovement.HasEntered == false)
                return false;
        }

        if (laneXPositions == null)
            return false;

        if (laneXPositions.Length == 0)
            return false;

        return true;
    }

    private void StartLaneMovement()
    {
        CalculateCameraBounds();

        hasStartedMovement = true;
        homeY = rb.position.y;

        currentLaneIndex = GetClosestLaneIndex();
        ChooseNextLane();

        isWaiting = false;
    }

    private int GetClosestLaneIndex()
    {
        int closestIndex = 0;
        float firstLaneX = GetClampedX(laneXPositions[0]);
        float closestDistance = Mathf.Abs(rb.position.x - firstLaneX);

        for (int i = 1; i < laneXPositions.Length; i++)
        {
            float laneX = GetClampedX(laneXPositions[i]);
            float distance = Mathf.Abs(rb.position.x - laneX);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    private void HandleWait()
    {
        waitTimer -= Time.fixedDeltaTime;

        if (waitTimer > 0f)
            return;

        ChooseNextLane();
        isWaiting = false;
    }

    private void ChooseNextLane()
    {
        if (laneXPositions.Length == 1)
        {
            currentLaneIndex = 0;
            targetX = GetClampedX(laneXPositions[0]);
            return;
        }

        int nextLaneIndex = currentLaneIndex;

        while (nextLaneIndex == currentLaneIndex)
        {
            nextLaneIndex = Random.Range(0, laneXPositions.Length);
        }

        currentLaneIndex = nextLaneIndex;
        targetX = GetClampedX(laneXPositions[currentLaneIndex]);
    }

    private void MoveToLane()
    {
        Vector2 currentPosition = rb.position;

        Vector2 targetPosition = new Vector2(
            targetX,
            homeY
        );

        float speed = GetCurrentMoveSpeed();

        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            speed * Time.fixedDeltaTime
        );

        nextPosition.x = GetClampedX(nextPosition.x);

        rb.MovePosition(nextPosition);

        float distance = Vector2.Distance(nextPosition, targetPosition);

        if (distance <= 0.02f)
        {
            targetPosition.x = GetClampedX(targetPosition.x);
            rb.MovePosition(targetPosition);

            waitTimer = GetCurrentWaitTime();
            isWaiting = true;
        }
    }

    private float GetCurrentMoveSpeed()
    {
        if (IsPhase2() == true)
            return phase2MoveSpeed;

        return moveSpeed;
    }

    private float GetCurrentWaitTime()
    {
        if (IsPhase2() == true)
        {
            return Random.Range(phase2MinWaitTime, phase2MaxWaitTime);
        }

        return Random.Range(minWaitTime, maxWaitTime);
    }

    private float GetClampedX(float xPosition)
    {
        if (clampInsideCamera == false)
            return xPosition;

        return Mathf.Clamp(xPosition, minX, maxX);
    }

    private void CalculateCameraBounds()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Boss2LaneMovement: Main Camera not found.");
            return;
        }

        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = halfHeight * mainCamera.aspect;

        float bossHalfWidth = 0f;

        if (bossCollider != null)
        {
            bossHalfWidth = bossCollider.bounds.extents.x;
        }

        minX = mainCamera.transform.position.x - halfWidth + horizontalPadding + bossHalfWidth;
        maxX = mainCamera.transform.position.x + halfWidth - horizontalPadding - bossHalfWidth;
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