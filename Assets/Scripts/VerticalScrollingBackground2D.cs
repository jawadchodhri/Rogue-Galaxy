using UnityEngine;

public sealed class VerticalScrollingBackground2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform backgroundA;
    [SerializeField] private Transform backgroundB;

    [Header("Movement")]
    [SerializeField] private float scrollSpeed = 2f;

    [Header("Fit")]
    [SerializeField] private bool autoFitToCamera = true;
    [SerializeField] private float extraScale = 1.08f;

    [Header("Loop")]
    [SerializeField] private float overlap = 0.08f;

    [Header("Position")]
    [SerializeField] private float zPosition = 0f;

    private SpriteRenderer rendererA;
    private SpriteRenderer rendererB;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        EnsureSecondBackground();
        CacheRenderers();

        if (rendererA == null || rendererB == null)
        {
            Debug.LogError("VerticalScrollingBackground2D: Missing SpriteRenderer.");
            enabled = false;
            return;
        }

        FitToCamera();  
        ResetBackgrounds();
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        MoveBackgrounds();
        LockXToCamera();
        RecycleIfNeeded();
    }

    private void EnsureSecondBackground()
    {
        if (backgroundA == null)
            return;

        if (backgroundB != null)
            return;

        // backgroundB = Instantiate(backgroundA, backgroundA.parent);
        backgroundB.name = backgroundA.name + "_Loop";
    }

    private void CacheRenderers()
    {
        if (backgroundA != null)
        {
            rendererA = backgroundA.GetComponent<SpriteRenderer>();
        }

        if (backgroundB != null)
        {
            rendererB = backgroundB.GetComponent<SpriteRenderer>();
        }
    }

    private void FitToCamera()
    {
        if (autoFitToCamera == false)
            return;

        if (rendererA.sprite == null)
            return;

        float cameraHeight = targetCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * targetCamera.aspect;

        Vector2 spriteSize = rendererA.sprite.bounds.size;

        if (spriteSize.x <= 0f || spriteSize.y <= 0f)
            return;

        float scale = Mathf.Max(cameraWidth / spriteSize.x, cameraHeight / spriteSize.y) * extraScale;

        Vector3 finalScale = new Vector3(scale, scale, 1f);

        backgroundA.localScale = finalScale;
        backgroundB.localScale = finalScale;
    }

    private void ResetBackgrounds()
    {
        Vector3 cameraCenter = targetCamera.transform.position;
        cameraCenter.z = zPosition;

        SetRendererCenter(rendererA, cameraCenter);

        backgroundB.position = backgroundA.position;
        PositionRendererAbove(rendererB, rendererA);
    }

    private void MoveBackgrounds()
    {
        float moveAmount = scrollSpeed * Time.deltaTime;

        backgroundA.position += Vector3.down * moveAmount;
        backgroundB.position += Vector3.down * moveAmount;
    }

    private void LockXToCamera()
    {
        float cameraX = targetCamera.transform.position.x;

        LockRendererX(rendererA, cameraX);
        LockRendererX(rendererB, cameraX);
    }

    private void RecycleIfNeeded()
    {
        float cameraBottom = targetCamera.transform.position.y - targetCamera.orthographicSize;

        if (rendererA.bounds.max.y < cameraBottom)
        {
            PositionRendererAbove(rendererA, rendererB);
        }

        if (rendererB.bounds.max.y < cameraBottom)
        {
            PositionRendererAbove(rendererB, rendererA);
        }
    }

    private void SetRendererCenter(SpriteRenderer renderer, Vector3 targetCenter)
    {
        Vector3 delta = targetCenter - renderer.bounds.center;

        Vector3 position = renderer.transform.position + delta;
        position.z = zPosition;

        renderer.transform.position = position;
    }

    private void LockRendererX(SpriteRenderer renderer, float targetX)
    {
        float deltaX = targetX - renderer.bounds.center.x;

        Vector3 position = renderer.transform.position;
        position.x += deltaX;
        position.z = zPosition;

        renderer.transform.position = position;
    }

    private void PositionRendererAbove(SpriteRenderer target, SpriteRenderer other)
    {
        Vector3 position = target.transform.position;
        position.x = other.transform.position.x;
        position.z = zPosition;
        target.transform.position = position;

        float desiredMinY = other.bounds.max.y - overlap;
        float deltaY = desiredMinY - target.bounds.min.y;

        position = target.transform.position;
        position.y += deltaY;
        position.z = zPosition;

        target.transform.position = position;
    }
}