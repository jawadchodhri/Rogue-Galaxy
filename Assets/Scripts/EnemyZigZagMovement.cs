using Unity.VisualScripting;
using UnityEngine;

public class EnemyZigZagMovement : MonoBehaviour
{
    [SerializeField] private float downSpeed = 2f;
    [SerializeField] private float sideSpeed = 2f;
    [SerializeField] private float horizontalPadding = 0.4f;

    private float direction = 1f;
    private float startX;
    private float minX;
    private float maxX;

    private void Start()
    {
        SetupCameraBounds();

        direction = Random.value < 0.5f ? -1f : 1f;
    }

    private void OnEnable()
    {
        startX = transform.position.x;
    }

    private void Update()
    {
        Movement();
    }

    private void SetupCameraBounds()
    {
        Camera cam = Camera.main;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        minX = cam.transform.position.x - halfWidth + horizontalPadding;
        maxX = cam.transform.position.x + halfWidth - horizontalPadding;
    }

    private void Movement()
    {
        Vector3 pos = transform.position;

        pos.y -= downSpeed * Time.deltaTime;
        pos.x += direction * sideSpeed * Time.deltaTime;

        if (pos.x <= minX)
        {
            pos.x = minX;
            direction = 1f;
        }
        else if (pos.x >= maxX)
        {
            pos.x = maxX;
            direction = -1f;
        }

        transform.position = pos;
    }
}
