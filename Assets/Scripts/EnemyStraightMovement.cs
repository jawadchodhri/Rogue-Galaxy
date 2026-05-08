using UnityEngine;

public class EnemyStraightMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float downSpeed;
    [SerializeField] private float chaseSpeed = 1.2f;

    [Header("Bounds")]
    [SerializeField] private float horizontalPadding = 0.4f;
    [SerializeField] private float bottomPadding = 1f;

    private Transform player;
    private EnemyWaveSpawner spawner;
    private Camera cam;

    private float minX;
    private float maxX;
    private float bottomY;

    private void Awake()
    {
        cam = Camera.main;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
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

    private void Movement()
    {
        Vector3 pos = transform.position;

        pos.y -= downSpeed * Time.deltaTime;

        if (player != null)
        {
            float targetX = player.position.x;
            pos.x = Mathf.MoveTowards(pos.x, targetX, chaseSpeed * Time.deltaTime);
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        transform.position = pos;
    }

    private void CheckBottomLimit()
    {
        if (transform.position.y < bottomY)
        {
            if (spawner != null)
                transform.position = spawner.GetRandomSpawnPosition();
        }
    }

    private void SetupCameraBounds()
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        minX = cam.transform.position.x - halfWidth + horizontalPadding;
        maxX = cam.transform.position.x + halfWidth - horizontalPadding;
        bottomY = cam.transform.position.y - halfHeight - bottomPadding;
    }
    
}
