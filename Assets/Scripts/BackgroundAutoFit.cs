using UnityEngine;

public class BackgroundAutoFit : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private SpriteRenderer spriteRenderer;
 
    [Header("Position")]
    [SerializeField] private bool followCameraXY = true;
    [SerializeField] private float backgroundZ = 5f;
 
    [Header("Coverage")]
    [SerializeField] private float zoomMultiplier = 1.05f;
    [SerializeField] private float widthOffset = 0.25f;
    [SerializeField] private float heightOffset = 0.25f;
 
    private Transform cachedTransform;
 
    private void Awake()
    {
        cachedTransform = transform;
 
        if (targetCamera == null)
            targetCamera = Camera.main;
 
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
 
    public void FitToCamera()
    {
        if (targetCamera == null || spriteRenderer == null)
            return;
 
        if (!targetCamera.orthographic)
            return;
 
        Sprite sprite = spriteRenderer.sprite;
        if (sprite == null)
            return;
 
        float cameraHeight = targetCamera.orthographicSize * 2f + heightOffset;
        float cameraWidth = cameraHeight * targetCamera.aspect + widthOffset;
 
        Vector2 spriteSize = sprite.bounds.size;
        if (spriteSize.x <= 0f || spriteSize.y <= 0f)
            return;
 
        float scaleX = cameraWidth / spriteSize.x;
        float scaleY = cameraHeight / spriteSize.y;
        float finalScale = Mathf.Max(scaleX, scaleY) * zoomMultiplier;
 
        cachedTransform.localScale = new Vector3(finalScale, finalScale, 1f);
 
        if (followCameraXY)
        {
            Vector3 camPos = targetCamera.transform.position;
            cachedTransform.position = new Vector3(camPos.x, camPos.y, backgroundZ);
        }
        else
        {
            Vector3 pos = cachedTransform.position;
            cachedTransform.position = new Vector3(pos.x, pos.y, backgroundZ);
        }
    }
 
    [ContextMenu("Fit Background")]
    private void FitBackgroundEditor()
    {
        FitToCamera();
    }
}