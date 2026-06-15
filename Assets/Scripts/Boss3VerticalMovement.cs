using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class Boss3VerticalMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private Boss3SpreadAttack spreadAttack;
    [SerializeField] private Camera targetCamera;

    [Header("Spawn")]
    [SerializeField] private bool startAboveCamera = true;
    [SerializeField] private float spawnAbovePadding = 1.5f;
    [SerializeField] private float xOffset = 0f;

    [Header("Positions")]
    [SerializeField] private float topStopY = 3.2f;
    [SerializeField] private float centerStopY = 0.6f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float phase2MoveSpeed = 3.8f;
    [SerializeField] private float arriveDistance = 0.03f;

    [Header("Attack Timing")]
    [SerializeField] private float pauseBeforeAttack = 0.25f;
    [SerializeField] private float pauseAfterAttack = 0.6f;
    [SerializeField] private float phase2PauseAfterAttack = 0.35f;

    private Rigidbody2D rb;
    private Coroutine movementRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (bossHealth == null)
        {
            bossHealth = GetComponent<BossHealth>();
        }

        if (spreadAttack == null)
        {
            spreadAttack = GetComponent<Boss3SpreadAttack>();
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        rb.gravityScale = 0f;
    }

    private void OnEnable()
    {
        if (startAboveCamera == true)
        {
            PlaceAboveCamera();
        }

        movementRoutine = StartCoroutine(MovementRoutine());
    }

    private void OnDisable()
    {
        if (movementRoutine != null)
        {
            StopCoroutine(movementRoutine);
            movementRoutine = null;
        }
    }

    private IEnumerator MovementRoutine()
    {
        Vector2 topPosition = GetTopPosition();
        Vector2 centerPosition = GetCenterPosition();

        yield return MoveToPosition(topPosition);

        while (true)
        {
            yield return DoAttackPause();

            yield return MoveToPosition(centerPosition);
            yield return DoAttackPause();

            yield return MoveToPosition(topPosition);
            yield return DoAttackPause();
        }
    }

    private IEnumerator MoveToPosition(Vector2 targetPosition)
    {
        while (Vector2.Distance(rb.position, targetPosition) > arriveDistance)
        {
            float speed = GetCurrentMoveSpeed();

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

    private IEnumerator DoAttackPause()
    {
        yield return new WaitForSeconds(pauseBeforeAttack);

        if (spreadAttack != null)
        {
            spreadAttack.ShootSpread();
        }

        yield return new WaitForSeconds(GetCurrentPauseAfterAttack());
    }

    private void PlaceAboveCamera()
    {
        if (targetCamera == null)
            return;

        float cameraTopY = targetCamera.transform.position.y + targetCamera.orthographicSize;

        Vector2 startPosition = new Vector2(
            GetCameraCenterX(),
            cameraTopY + spawnAbovePadding
        );

        rb.position = startPosition;
    }

    private Vector2 GetTopPosition()
    {
        return new Vector2(
            GetCameraCenterX(),
            topStopY
        );
    }

    private Vector2 GetCenterPosition()
    {
        return new Vector2(
            GetCameraCenterX(),
            centerStopY
        );
    }

    private float GetCameraCenterX()
    {
        if (targetCamera == null)
            return transform.position.x;

        return targetCamera.transform.position.x + xOffset;
    }

    private float GetCurrentMoveSpeed()
    {
        if (IsPhase2() == true)
            return phase2MoveSpeed;

        return moveSpeed;
    }

    private float GetCurrentPauseAfterAttack()
    {
        if (IsPhase2() == true)
            return phase2PauseAfterAttack;

        return pauseAfterAttack;
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