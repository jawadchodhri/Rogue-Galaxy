using UnityEngine;

public class BossEntryMovement : MonoBehaviour
{
    [SerializeField] private float entrySpeed = 0.8f;
    [SerializeField] private float topScreenOffset = 1.5f;

    private Camera cam;
    private float stopY;
    private bool hasEntered;

    public bool HasEntered => hasEntered;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Start()
    {
        float topY = cam.transform.position.y + cam.orthographicSize;
        stopY = topY - topScreenOffset;
    }

    private void Update()
    {
        if (hasEntered) return;

        Vector3 pos = transform.position;

        pos.y = Mathf.MoveTowards(
            pos.y,
            stopY,
            entrySpeed * Time.deltaTime
        );

        transform.position = pos;

        if (Mathf.Abs(pos.y - stopY) <= 0.01f)
        {
            hasEntered = true;
            transform.position = new Vector3(pos.x, stopY, pos.z);
        }
    }
}