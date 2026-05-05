using Unity.VisualScripting;
using UnityEngine;

public class EnemyZigZagMovement : MonoBehaviour
{
    [SerializeField] private float downSpeed = 2f;
    [SerializeField] private float sideSpeed = 2f;
    [SerializeField] private float horizontalPadding = 0.4f;
    [SerializeField] private float bottomPadding = 1f;

    private EnemyWaveSpawner spawner;
    private Camera cam;

    private float direction = 1f;
    private float minX;
    private float maxX;
    private float bottomY;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Start()
    {
        SetupCameraBounds();
        RandomizeDirection();
    }

    public void Initialize(EnemyWaveSpawner owner)
    {
        spawner = owner;
    }

    private void Update()
    {
        Movement();
        CheckBottomLimit();
    }

    private void SetupCameraBounds()
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        minX = cam.transform.position.x - halfWidth + horizontalPadding;
        maxX = cam.transform.position.x + halfWidth - horizontalPadding;
        bottomY = cam.transform.position.y - halfHeight - bottomPadding;
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

    private void CheckBottomLimit()
    {
        if (transform.position.y < bottomY)
        {
            if (spawner == null)
            {
                return;
            }
            else
            {
                transform.position = spawner.GetRandomSpawnPosition();
                RandomizeDirection();
            }
        }
    }

    private void RandomizeDirection()
    {
        direction = Random.value < 0.5f ? -1f : 1f;
    }
}
