using UnityEngine;

public class EnemyVisibilityGate : MonoBehaviour
{
    private float topPadding = 0.8f;

    private Camera cam;
    private bool hasEnteredCamera;

    public bool HasEnteredCamera => hasEnteredCamera;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (hasEnteredCamera) return;

        float cameraTopY = cam.transform.position.y + cam.orthographicSize;

        if (transform.position.y <= cameraTopY - topPadding)
        {
            hasEnteredCamera = true;
        }
    }

    public void ResetGate()
    {
        hasEnteredCamera = false;
    }
}