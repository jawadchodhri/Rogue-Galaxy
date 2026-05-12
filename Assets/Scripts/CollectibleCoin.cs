using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    // [SerializeField] private int coinValue = 1;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float lifeTime = 8f;

    private CoinPatternSpawner owner;
    private bool countedRemoved;

    public void Initialize(CoinPatternSpawner spawner)
    {
        owner = spawner;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player == null)
            player = other.GetComponentInParent<PlayerHealth>();

        if (player == null) return;

        RemoveCoin();
    }

    private void OnDestroy()
    {
        NotifyRemoved();
    }

    private void RemoveCoin()
    {
        NotifyRemoved();
        Destroy(gameObject);
    }

    private void NotifyRemoved()
    {
        if (countedRemoved) return;

        countedRemoved = true;

        if (owner != null)
            owner.OnCoinRemoved();
    }
}