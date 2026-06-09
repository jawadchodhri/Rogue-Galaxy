using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class DashWarningVisual : MonoBehaviour
{
    [Header("Auto Fit")]
    [SerializeField] private bool fitToCameraHeight = true;
    [SerializeField] private float width = 0.7f;
    [SerializeField] private float heightMultiplier = 1.1f;

    [Header("Pulse")]
    [SerializeField] private float pulseSpeed = 12f;
    [SerializeField] private float minAlpha = 0.25f;
    [SerializeField] private float maxAlpha = 0.75f;

    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        mainCamera = Camera.main;

        FitToCameraHeight();
    }

    private void Update()
    {
        float alpha = Mathf.Lerp(
            minAlpha,
            maxAlpha,
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f
        );

        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    private void FitToCameraHeight()
    {
        if (fitToCameraHeight == false)
            return;

        if (mainCamera == null)
            return;

        float cameraHeight = mainCamera.orthographicSize * 2f;
        float targetHeight = cameraHeight * heightMultiplier;

        transform.localScale = new Vector3(
            width,
            targetHeight,
            1f
        );
    }
}