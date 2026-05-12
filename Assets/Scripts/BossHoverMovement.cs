using UnityEngine;

public class BossHoverMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossEntryMovement entryMovement;
    [SerializeField] private BossHealth bossHealth;

    [Header("Phase 1")]
    [SerializeField] private float horizontalAmount = 1.2f;
    [SerializeField] private float horizontalSpeed = 0.7f;
    [SerializeField] private float verticalAmount = 0.12f;
    [SerializeField] private float verticalSpeed = 1.8f;

    [Header("Phase 2")]
    [SerializeField] private float phase2HorizontalAmount = 1.8f;
    [SerializeField] private float phase2HorizontalSpeed = 1.2f;
    [SerializeField] private float phase2VerticalAmount = 0.2f;
    [SerializeField] private float phase2VerticalSpeed = 2.5f;

    [Header("Bounds")]
    [SerializeField] private float leftOffset = 2f;
    [SerializeField] private float rightOffset = 2f;

    private Camera mainCamera;

    private float minX;
    private float maxX;

    private float hoverTimer;

    private Vector3 centerPosition;
    private bool hasSetCenter;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (entryMovement == null)
            entryMovement = GetComponent<BossEntryMovement>();

        if (bossHealth == null)
            bossHealth = GetComponent<BossHealth>();
    }

    private void Start()
    {
        SetupBounds();
    }

    private void Update()
    {
        if (entryMovement != null && !entryMovement.HasEntered)
            return;

        if (!hasSetCenter)
        {
            centerPosition = transform.position;
            hasSetCenter = true;
        }

        bool phase2 = bossHealth != null && bossHealth.IsPhase2;

        float hAmount = phase2 ? phase2HorizontalAmount : horizontalAmount;
        float hSpeed = phase2 ? phase2HorizontalSpeed : horizontalSpeed;
        float vAmount = phase2 ? phase2VerticalAmount : verticalAmount;
        float vSpeed = phase2 ? phase2VerticalSpeed : verticalSpeed;

        hoverTimer += Time.deltaTime;

        float xOffset = Mathf.Sin(hoverTimer * hSpeed) * hAmount;
        float yOffset = Mathf.Sin(hoverTimer * vSpeed) * vAmount;

        Vector3 targetPos = centerPosition + new Vector3(xOffset, yOffset, 0f);

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);

        transform.position = targetPos;
    }

    private void SetupBounds()
    {
        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = halfHeight * mainCamera.aspect;

        minX = mainCamera.transform.position.x - halfWidth + leftOffset;
        maxX = mainCamera.transform.position.x + halfWidth - rightOffset;
    }
}