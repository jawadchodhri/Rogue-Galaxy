using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class BackgroundAutoFit2D : MonoBehaviour
{
    public enum FitMode
    {
        Cover,   // Fills the whole screen, may crop edges
        Contain  // Shows full background, may show empty bars
    }

    [Header("References")]
    [SerializeField] private Camera targetCamera;

    [Header("Fit Settings")]
    [SerializeField] private FitMode fitMode = FitMode.Cover;

    [Tooltip("Extra scale after fitting. Use 1.05 if tiny edges appear.")]
    [SerializeField] private float scaleMultiplier = 1f;

    [Header("Position")]
    [SerializeField] private bool followCameraPosition = true;
    [SerializeField] private float zPosition = 10f;

    private SpriteRenderer spriteRenderer;
    private Vector2 lastScreenSize;
    private float lastCameraSize;
    private float lastCameraAspect;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (targetCamera == null)
            targetCamera = Camera.main;

        FitToCamera();
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        if (followCameraPosition)
        {
            Vector3 camPos = targetCamera.transform.position;
            transform.position = new Vector3(camPos.x, camPos.y, zPosition);
        }

        float currentAspect = targetCamera.aspect;

        if (lastScreenSize.x != Screen.width ||
            lastScreenSize.y != Screen.height ||
            !Mathf.Approximately(lastCameraSize, targetCamera.orthographicSize) ||
            !Mathf.Approximately(lastCameraAspect, currentAspect))
        {
            FitToCamera();
        }
    }

    private void FitToCamera()
    {
        if (targetCamera == null || spriteRenderer.sprite == null)
            return;

        float cameraHeight = targetCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * targetCamera.aspect;

        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        float scaleX = cameraWidth / spriteSize.x;
        float scaleY = cameraHeight / spriteSize.y;

        float finalScale = fitMode == FitMode.Cover
            ? Mathf.Max(scaleX, scaleY)
            : Mathf.Min(scaleX, scaleY);

        finalScale *= scaleMultiplier;

        transform.localScale = new Vector3(finalScale, finalScale, 1f);

        lastScreenSize = new Vector2(Screen.width, Screen.height);
        lastCameraSize = targetCamera.orthographicSize;
        lastCameraAspect = targetCamera.aspect;
    }
}