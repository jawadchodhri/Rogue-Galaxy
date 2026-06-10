using UnityEngine;

public sealed class VerticalParallaxBackground2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform farGround;
    [SerializeField] private Transform farGround2;

    [Header("Movement")]
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private float overlapOffset = 0.05f;

    [Header("Auto Fit")]
    [SerializeField] private bool autoFitToCamera = true;
    [SerializeField] private float extraScale = 1.05f;

    [Header("Parallax")]
    [Tooltip("0 = world fixed, 0.5 = slow parallax, 1 = follows camera.")]
    [Range(0f, 1f)]
    [SerializeField] private float parallaxFactor = 0.35f;

    [Header("Position Offset")]
    [SerializeField] private float xOffset = 0f;
    [SerializeField] private float yOffset = 0f;
    [SerializeField] private float zPosition = 10f;

    [Header("Loop")]
    [SerializeField] private float verticalOverlap = 0.05f;
    [SerializeField] private float recycleBuffer = 1f;

    private SpriteRenderer farGroundRenderer;
    private SpriteRenderer farGround2Renderer;

    private float segmentHeight;
    private float previousCameraY;
    private float lastCameraSize;
    private float lastCameraAspect;
    private float height;

    private bool initialized;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        EnsureSecondBackground();
        CacheRenderers();

        if (farGroundRenderer == null || farGround2Renderer == null)
        {
            Debug.LogError($"{nameof(VerticalParallaxBackground2D)} needs SpriteRenderer on both background objects.", this);
            enabled = false;
            return;
        }

        height = farGroundRenderer.bounds.size.y;

        farGround2.position = new Vector3(
            farGround.position.x,
            farGround.position.y + height - overlapOffset,
            farGround.position.z
        );

        ResetBackground();
    }

    private void Start()
    {
    }

    private void Update()
    {
        BackGroundMovement();
    }

    private void LateUpdate()
    {
        if (targetCamera == null || farGround == null || farGround2 == null)
            return;

        RefreshFitIfNeeded();
        MoveWithParallax();
        LockXToCamera();
        RecycleBackgrounds();
    }

    private void EnsureSecondBackground()
    {
        if (farGround == null)
            return;

        if (farGround2 != null)
            return;

        farGround2 = Instantiate(farGround, farGround.parent);
        farGround2.name = farGround.name + "_Loop";
    }

    private void CacheRenderers()
    {
        if (farGround != null)
            farGroundRenderer = farGround.GetComponent<SpriteRenderer>();

        if (farGround2 != null)
            farGround2Renderer = farGround2.GetComponent<SpriteRenderer>();
    }

    private void RefreshFitIfNeeded()
    {
        if (!autoFitToCamera)
            return;

        if (Mathf.Approximately(lastCameraSize, targetCamera.orthographicSize) &&
            Mathf.Approximately(lastCameraAspect, targetCamera.aspect))
        {
            return;
        }

        FitToCamera();
        PositionSecondBackground();
    }

    private void FitToCamera()
    {
        if (targetCamera == null || farGroundRenderer == null || farGroundRenderer.sprite == null)
            return;

        float cameraHeight = targetCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * targetCamera.aspect;

        Vector2 spriteSize = farGroundRenderer.sprite.bounds.size;

        if (spriteSize.x <= 0f || spriteSize.y <= 0f)
            return;

        float scale = Mathf.Max(
            cameraWidth / spriteSize.x,
            cameraHeight / spriteSize.y
        ) * extraScale;

        Vector3 finalScale = new Vector3(scale, scale, 1f);

        farGround.localScale = finalScale;
        farGround2.localScale = finalScale;

        segmentHeight = farGroundRenderer.bounds.size.y;
        height = segmentHeight;

        lastCameraSize = targetCamera.orthographicSize;
        lastCameraAspect = targetCamera.aspect;
    }

    private void MoveWithParallax()
    {
        float cameraY = targetCamera.transform.position.y;
        float deltaY = cameraY - previousCameraY;
        float moveY = deltaY * parallaxFactor;

        MoveTransformY(farGround, moveY);
        MoveTransformY(farGround2, moveY);

        previousCameraY = cameraY;
    }

    private static void MoveTransformY(Transform target, float amount)
    {
        Vector3 position = target.position;
        position.y += amount;
        target.position = position;
    }

    private void LockXToCamera()
    {
        float targetX = targetCamera.transform.position.x + xOffset;

        Vector3 posA = farGround.position;
        posA.x = targetX;
        posA.z = zPosition;
        farGround.position = posA;

        Vector3 posB = farGround2.position;
        posB.x = targetX;
        posB.z = zPosition;
        farGround2.position = posB;
    }

    private void RecycleBackgrounds()
    {
        float cameraBottom = targetCamera.transform.position.y - targetCamera.orthographicSize - recycleBuffer;
        float cameraTop = targetCamera.transform.position.y + targetCamera.orthographicSize + recycleBuffer;

        float step = segmentHeight - verticalOverlap;

        if (step <= 0.001f)
            return;

        SnapAboveIfNeeded(farGroundRenderer, farGround2Renderer, cameraBottom, step);
        SnapAboveIfNeeded(farGround2Renderer, farGroundRenderer, cameraBottom, step);

        SnapBelowIfNeeded(farGroundRenderer, farGround2Renderer, cameraTop, step);
        SnapBelowIfNeeded(farGround2Renderer, farGroundRenderer, cameraTop, step);
    }

    private static void SnapAboveIfNeeded(SpriteRenderer target, SpriteRenderer other, float cameraBottom, float step)
    {
        float gap = cameraBottom - target.bounds.max.y;

        if (gap <= 0f)
            return;

        int jumps = Mathf.Max(1, Mathf.CeilToInt(gap / step));

        Vector3 position = target.transform.position;
        position.y = other.transform.position.y + step * jumps;
        target.transform.position = position;
    }

    private static void SnapBelowIfNeeded(SpriteRenderer target, SpriteRenderer other, float cameraTop, float step)
    {
        float gap = target.bounds.min.y - cameraTop;

        if (gap <= 0f)
            return;

        int jumps = Mathf.Max(1, Mathf.CeilToInt(gap / step));

        Vector3 position = target.transform.position;
        position.y = other.transform.position.y - step * jumps;
        target.transform.position = position;
    }

    private void PositionSecondBackground()
    {
        if (!initialized)
            return;

        float step = segmentHeight - verticalOverlap;

        Vector3 posA = farGround.position;
        Vector3 posB = farGround2.position;

        posB.x = posA.x;
        posB.y = posA.y + step;
        posB.z = posA.z;

        farGround2.position = posB;
    }

    public void ResetBackground()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null || farGround == null || farGround2 == null)
            return;

        FitToCamera();

        float cameraY = targetCamera.transform.position.y;
        float cameraX = targetCamera.transform.position.x + xOffset;

        Vector3 posA = new Vector3(cameraX, cameraY + yOffset, zPosition);
        Vector3 posB = new Vector3(cameraX, cameraY + yOffset + segmentHeight - verticalOverlap, zPosition);

        farGround.position = posA;
        farGround2.position = posB;

        previousCameraY = cameraY;
        initialized = true;
    }

    private void BackGroundMovement()
    {
        float moveAmount = scrollSpeed * Time.deltaTime;

        farGround.position += Vector3.down * moveAmount;
        farGround2.position += Vector3.down * moveAmount;

        if (farGround.position.y <= -height)
        {
            farGround.position = new Vector3(
                farGround.position.x,
                farGround2.position.y + height - overlapOffset,
                farGround.position.z
            );
        }

        if (farGround2.position.y <= -height)
        {
            farGround2.position = new Vector3(
                farGround2.position.x,
                farGround.position.y + height - overlapOffset,
                farGround2.position.z
            );
        }
    }
}