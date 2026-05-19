using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class MosquitoSideSlideMovement : MonoBehaviour
{
    private enum MoveState
    {
        Entering,
        WaitingToAttack,
        Exiting,
        WaitingToReEnter
    }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float moveY = 3.4f;
    [SerializeField] private float outsidePadding = 1f;

    [Header("Random Attack Position")]
    [SerializeField] private bool useRandomAttackX = true;
    [SerializeField] private float fixedAttackX = 0f;
    [SerializeField] private float attackSidePadding = 0.8f;

    [Header("Timing")]
    [SerializeField] private float attackPauseDuration = 1f;
    [SerializeField] private float reEnterDelay = 0.7f;

    [Header("Spawn Side")]
    [SerializeField] private bool randomStartSide = true;
    [SerializeField] private bool startFromLeft = true;

    public bool CanAttack
    {
        get { return canAttack; }
    }

    private Rigidbody2D rb;
    private Camera mainCamera;

    private MoveState currentState;

    private float leftOutsideX;
    private float rightOutsideX;

    private float leftAttackX;
    private float rightAttackX;

    private float targetAttackX;
    private float exitX;
    private float stateTimer;

    private int moveDirection;
    private bool canAttack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        rb.gravityScale = 0f;

        CalculateCameraBounds();
        SetupStartSide();
    }

    private void FixedUpdate()
    {
        if (currentState == MoveState.Entering)
        {
            MoveIntoScreen();
            return;
        }

        if (currentState == MoveState.Exiting)
        {
            MoveOutOfScreen();
            return;
        }
    }

    private void Update()
    {
        if (currentState == MoveState.WaitingToAttack)
        {
            HandleAttackPause();
            return;
        }

        if (currentState == MoveState.WaitingToReEnter)
        {
            HandleReEnterDelay();
        }
    }

    private void SetupStartSide()
    {
        if (randomStartSide == true)
        {
            int randomValue = Random.Range(0, 2);

            if (randomValue == 0)
            {
                startFromLeft = true;
            }
            else
            {
                startFromLeft = false;
            }
        }

        ChooseNewAttackX();

        if (startFromLeft == true)
        {
            moveDirection = 1;
            rb.position = new Vector2(leftOutsideX, moveY);
            exitX = rightOutsideX;
        }
        else
        {
            moveDirection = -1;
            rb.position = new Vector2(rightOutsideX, moveY);
            exitX = leftOutsideX;
        }

        canAttack = false;
        currentState = MoveState.Entering;
    }

    private void ChooseNewAttackX()
    {
        if (useRandomAttackX == true)
        {
            targetAttackX = Random.Range(leftAttackX, rightAttackX);
        }
        else
        {
            targetAttackX = fixedAttackX;
        }
    }

    private void MoveIntoScreen()
    {
        Vector2 currentPosition = rb.position;
        Vector2 nextPosition = currentPosition + Vector2.right * moveDirection * moveSpeed * Time.fixedDeltaTime;

        bool reachedAttackPosition = false;

        if (moveDirection == 1)
        {
            if (nextPosition.x >= targetAttackX)
            {
                reachedAttackPosition = true;
            }
        }
        else
        {
            if (nextPosition.x <= targetAttackX)
            {
                reachedAttackPosition = true;
            }
        }

        if (reachedAttackPosition == true)
        {
            nextPosition.x = targetAttackX;
            nextPosition.y = moveY;

            rb.MovePosition(nextPosition);

            canAttack = true;
            stateTimer = attackPauseDuration;
            currentState = MoveState.WaitingToAttack;
            return;
        }

        rb.MovePosition(nextPosition);
    }

    private void HandleAttackPause()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer > 0f)
            return;

        canAttack = false;
        currentState = MoveState.Exiting;
    }

    private void MoveOutOfScreen()
    {
        Vector2 currentPosition = rb.position;
        Vector2 nextPosition = currentPosition + Vector2.right * moveDirection * moveSpeed * Time.fixedDeltaTime;

        bool reachedExit = false;

        if (moveDirection == 1)
        {
            if (nextPosition.x >= exitX)
            {
                reachedExit = true;
            }
        }
        else
        {
            if (nextPosition.x <= exitX)
            {
                reachedExit = true;
            }
        }

        if (reachedExit == true)
        {
            nextPosition.x = exitX;
            nextPosition.y = moveY;

            rb.MovePosition(nextPosition);

            stateTimer = reEnterDelay;
            currentState = MoveState.WaitingToReEnter;
            return;
        }

        rb.MovePosition(nextPosition);
    }

    private void HandleReEnterDelay()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer > 0f)
            return;

        SwitchSideAndReEnter();
    }

    private void SwitchSideAndReEnter()
    {
        ChooseNewAttackX();

        if (moveDirection == 1)
        {
            moveDirection = -1;
            rb.position = new Vector2(rightOutsideX, moveY);
            exitX = leftOutsideX;
        }
        else
        {
            moveDirection = 1;
            rb.position = new Vector2(leftOutsideX, moveY);
            exitX = rightOutsideX;
        }

        canAttack = false;
        currentState = MoveState.Entering;
    }

    private void CalculateCameraBounds()
    {
        if (mainCamera == null)
        {
            Debug.LogError("MosquitoSideSlideMovement: Main Camera not found.");
            return;
        }

        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = halfHeight * mainCamera.aspect;

        float cameraX = mainCamera.transform.position.x;

        leftOutsideX = cameraX - halfWidth - outsidePadding;
        rightOutsideX = cameraX + halfWidth + outsidePadding;

        leftAttackX = cameraX - halfWidth + attackSidePadding;
        rightAttackX = cameraX + halfWidth - attackSidePadding;
    }
}