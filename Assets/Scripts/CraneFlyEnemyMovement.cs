using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class CraneFlyEnemyMovement : MonoBehaviour
{
    private enum MoveState
    {
        Entering,
        MovingToPoint,
        Pausing
    }

    [Header("Camera")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float horizontalPadding = 0.45f;
    [SerializeField] private float topSpawnPadding = 1f;

    [Header("Hover Area")]
    [Range(0f, 1f)]
    [SerializeField] private float hoverMinYNormalized = 0.45f;

    [Range(0f, 1f)]
    [SerializeField] private float hoverMaxYNormalized = 0.82f;

    [Header("Movement")]
    [SerializeField] private float enterSpeed = 2.4f;
    [SerializeField] private float moveSpeed = 1.8f;
    [SerializeField] private float arriveDistance = 0.08f;

    [Header("Pause")]
    [SerializeField] private float minPauseDuration = 0.25f;
    [SerializeField] private float maxPauseDuration = 0.6f;

    [Header("Unstable Flutter")]
    [SerializeField] private bool useFlutter = true;
    [SerializeField] private float flutterChangeInterval = 0.18f;
    [SerializeField] private float flutterStrength = 0.18f;

    public bool CanAttack
    {
        get { return canAttack; }
    }

    private Rigidbody2D rb;

    private MoveState currentState;

    private float minX;
    private float maxX;
    private float bottomY;
    private float topY;
    private float hoverMinY;
    private float hoverMaxY;

    private float pauseTimer;
    private float flutterTimer;

    private Vector2 targetPoint;
    private Vector2 flutterOffset;

    private bool canAttack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        rb.gravityScale = 0f;

        CalculateCameraBounds();
        SpawnAboveCamera();
    }

    private void FixedUpdate()
    {
        if (targetCamera == null)
            return;

        CalculateCameraBounds();

        if (currentState == MoveState.Entering)
        {
            MoveToEntryPoint();
            return;
        }

        if (currentState == MoveState.MovingToPoint)
        {
            MoveToHoverPoint();
            return;
        }

        if (currentState == MoveState.Pausing)
        {
            HandlePause();
        }
    }

    private void MoveToEntryPoint()
    {
        Vector2 currentPosition = rb.position;

        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            targetPoint,
            enterSpeed * Time.fixedDeltaTime
        );

        nextPosition.x = Mathf.Clamp(nextPosition.x, minX, maxX);

        rb.MovePosition(nextPosition);

        float distance = Vector2.Distance(nextPosition, targetPoint);

        if (distance <= arriveDistance)
        {
            rb.MovePosition(targetPoint);
            canAttack = true;
            StartPause();
        }
    }

    private void MoveToHoverPoint()
    {
        UpdateFlutter();

        Vector2 finalTarget = targetPoint + flutterOffset;
        finalTarget.x = Mathf.Clamp(finalTarget.x, minX, maxX);
        finalTarget.y = Mathf.Clamp(finalTarget.y, hoverMinY, hoverMaxY);

        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            finalTarget,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);

        float distance = Vector2.Distance(rb.position, targetPoint);

        if (distance <= arriveDistance)
        {
            StartPause();
        }
    }

    private void HandlePause()
    {
        pauseTimer -= Time.fixedDeltaTime;

        if (pauseTimer > 0f)
            return;

        PickNewHoverPoint();
        currentState = MoveState.MovingToPoint;
    }

    private void StartPause()
    {
        pauseTimer = Random.Range(minPauseDuration, maxPauseDuration);
        currentState = MoveState.Pausing;
    }

    private void PickNewHoverPoint()
    {
        targetPoint = new Vector2(
            Random.Range(minX, maxX),
            Random.Range(hoverMinY, hoverMaxY)
        );
    }

    private void UpdateFlutter()
    {
        if (useFlutter == false)
        {
            flutterOffset = Vector2.zero;
            return;
        }

        flutterTimer -= Time.fixedDeltaTime;

        if (flutterTimer > 0f)
            return;

        flutterTimer = flutterChangeInterval;

        flutterOffset = new Vector2(
            Random.Range(-flutterStrength, flutterStrength),
            Random.Range(-flutterStrength, flutterStrength)
        );
    }

    private void SpawnAboveCamera()
    {
        Vector2 spawnPosition = new Vector2(
            Random.Range(minX, maxX),
            topY + topSpawnPadding
        );

        rb.position = spawnPosition;

        targetPoint = new Vector2(
            spawnPosition.x,
            Random.Range(hoverMinY, hoverMaxY)
        );

        canAttack = false;
        currentState = MoveState.Entering;
    }

    private void CalculateCameraBounds()
    {
        if (targetCamera == null)
        {
            Debug.LogError("CraneFlyEnemyMovement: Camera not found.");
            return;
        }

        float halfHeight = targetCamera.orthographicSize;
        float halfWidth = halfHeight * targetCamera.aspect;

        Vector3 cameraPosition = targetCamera.transform.position;

        minX = cameraPosition.x - halfWidth + horizontalPadding;
        maxX = cameraPosition.x + halfWidth - horizontalPadding;

        bottomY = cameraPosition.y - halfHeight;
        topY = cameraPosition.y + halfHeight;

        hoverMinY = Mathf.Lerp(bottomY, topY, hoverMinYNormalized);
        hoverMaxY = Mathf.Lerp(bottomY, topY, hoverMaxYNormalized);
    }
}