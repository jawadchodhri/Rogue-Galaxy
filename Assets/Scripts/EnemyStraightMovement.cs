using UnityEngine;

public class EnemyStraightMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float horizontalPadding = 0.4f;

    private float minX;
    private float maxX;

    private void Start()
    {
        SetupCameraBounds();
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

        pos += Vector3.down * moveSpeed * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        transform.position = pos;
    }
}
