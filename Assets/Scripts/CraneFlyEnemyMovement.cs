using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class CraneFlyEnemyMovement : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool repositionAboveCameraOnStart = true;
    [SerializeField] private float horizontalPadding = 0.45f;
    [SerializeField] private float topSpawnPadding = 1f;
    [SerializeField] private float bottomExitPadding = 1f;

    [Header("Downward Drift")]
    [SerializeField] private float downSpeed = 1.4f;

    [Header("S-Curve Sway")]
    [SerializeField] private float swayAmplitude = 1.2f;
    [SerializeField] private float swaySpeed = 2f;

    [Header("Gentle Flutter")]
    [SerializeField] private float verticalFlutterAmplitude = 0.12f;
    [SerializeField] private float verticalFlutterSpeed = 4f;

    [Header("Random Short Pauses")]
    [SerializeField] private bool useRandomPauses = true;
    [SerializeField] private float minPauseInterval = 1.5f;
    [SerializeField] private float maxPauseInterval = 3f;
    [SerializeField] private float minPauseDuration = 0.15f;
    [SerializeField] private float maxPauseDuration = 0.35f;

    public bool CanAttack
    {
        get { return canAttack; }
    }

    private Rigidbody2D rb;

    private float minX;
    private float maxX;
    private float topY;
    private float bottomY;

    private float centerX;
    private float baseY;
    private float randomSeed;

    private float nextPauseTime;
    private float pauseTimer;

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

        randomSeed = Random.Range(0f, 100f);

        if (repositionAboveCameraOnStart == true)
        {
            RepositionAboveCamera();
        }
        else
        {
            Vector2 currentPosition = rb.position;
            centerX = Mathf.Clamp(currentPosition.x, minX, maxX);
            baseY = currentPosition.y;
        }

        ScheduleNextPause();
    }

    private void FixedUpdate()
    {
        if (targetCamera == null)
            return;

        HandlePauseTimer();
        MoveWithDrift();
        UpdateCanAttackState();
        CheckBottomExit();
    }

    private void MoveWithDrift()
    {
        if (pauseTimer <= 0f)
        {
            baseY -= downSpeed * Time.fixedDeltaTime;
        }

        float swayOffset = Mathf.Sin((Time.time + randomSeed) * swaySpeed) * swayAmplitude;
        float flutterOffset = Mathf.Sin((Time.time + randomSeed) * verticalFlutterSpeed) * verticalFlutterAmplitude;

        Vector2 targetPosition = new Vector2(
            centerX + swayOffset,
            baseY + flutterOffset
        );

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);

        rb.MovePosition(targetPosition);
    }

    private void HandlePauseTimer()
    {
        if (useRandomPauses == false)
            return;

        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.fixedDeltaTime;

            if (pauseTimer <= 0f)
            {
                ScheduleNextPause();
            }

            return;
        }

        if (Time.time < nextPauseTime)
            return;

        pauseTimer = Random.Range(minPauseDuration, maxPauseDuration);
    }

    private void ScheduleNextPause()
    {
        nextPauseTime = Time.time + Random.Range(minPauseInterval, maxPauseInterval);
    }

    private void UpdateCanAttackState()
    {
        if (rb.position.y <= topY)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
    }

    private void CheckBottomExit()
    {
        if (rb.position.y > bottomY - bottomExitPadding)
            return;

        RepositionAboveCamera();
        ScheduleNextPause();
    }

    private void RepositionAboveCamera()
    {
        CalculateCameraBounds();

        centerX = Random.Range(minX, maxX);
        baseY = topY + topSpawnPadding;

        Vector2 spawnPosition = new Vector2(
            centerX,
            baseY
        );

        rb.position = spawnPosition;
        canAttack = false;
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
        topY = cameraPosition.y + halfHeight;
        bottomY = cameraPosition.y - halfHeight;
    }
}