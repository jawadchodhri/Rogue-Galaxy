using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private Transform farGround;
    [SerializeField] private Transform farGround2;
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private float overlapOffset = 0.05f;

    private float height;

    private void Awake()
    {
        height = farGround.GetComponent<SpriteRenderer>().bounds.size.y;

        farGround2.position = new Vector3(farGround.position.x, farGround.position.y + height - overlapOffset, farGround.position.z);
    }

    private void Update()
    {
        BackGroundMovement();
    }

    private void BackGroundMovement()
    {
        float moveAmount = scrollSpeed * Time.deltaTime;

        farGround.position += Vector3.down * moveAmount;
        farGround2.position += Vector3.down * moveAmount;

        if (farGround.position.y <= -height)
            farGround.position = new Vector3(farGround.position.x, farGround2.position.y + height - overlapOffset, farGround.position.z);

        if (farGround2.position.y <= -height)
            farGround2.position = new Vector3(farGround2.position.x, farGround.position.y + height - overlapOffset, farGround2.position.z);
    }
}
