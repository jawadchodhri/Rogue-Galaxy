using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    // [SerializeField] private int coinValue = 1;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float lifeTime = 8f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("Coin collected");

        Destroy(gameObject);
    }
}