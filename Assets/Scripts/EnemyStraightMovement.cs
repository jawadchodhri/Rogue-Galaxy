using UnityEngine;

public class EnemyStraightMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float horizontalPadding = 0.4f;
    [SerializeField] private float bottomPadding = 1f;

    private Camera cam;
    private EnemyWaveSpawner spawner;

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

        pos += Vector3.down * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        transform.position = pos;
    }

    private void CheckBottomLimit()
    {
        if (transform.position.y < bottomY)
        {
            if (spawner != null)
            {
                transform.position = spawner.GetRandomSpawnPosition();
                Debug.Log("Bugs Enemy reappeared at top: " + transform.position);
            }
        }
    }
}
