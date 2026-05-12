using System.Collections;
using UnityEngine;

public class CoinPatternSpawner : MonoBehaviour
{
    private enum PatternType
    {
        Line,
        ZigZag,
        Triangle,
        Box,
        Circle
    }

    [Header("Coin")]
    [SerializeField] private CoinCollectible coinPrefab;

    [Header("Pattern")]
    [SerializeField] private PatternType[] allowedPatterns;
    [SerializeField] private int coinsPerPattern = 6;
    [SerializeField] private float spacing = 1f;
    [SerializeField] private float radius = 1.8f;

    [Header("Spawn")]
    [SerializeField] private float spawnY = 6f;
    [SerializeField] private float patternSpawnDelay = 2.2f;
    [SerializeField] private float horizontalPadding = 1.8f;

    private Camera cam;
    private float minX;
    private float maxX;

    private Coroutine routine;
    private int activeCoins;

    public bool HasActiveCoins => activeCoins > 0;

    private void Awake()
    {
        cam = Camera.main;
        CalculateBounds();
    }

    public void StartPatterns()
    {
        if (routine != null) return;

        routine = StartCoroutine(PatternRoutine());
    }

    public void StopPatterns()
    {
        if (routine == null) return;

        StopCoroutine(routine);
        routine = null;
    }

    public void OnCoinRemoved()
    {
        activeCoins--;
        if (activeCoins < 0) activeCoins = 0;
    }

    private IEnumerator PatternRoutine()
    {
        while (true)
        {
            SpawnRandomPattern();
            yield return new WaitForSeconds(patternSpawnDelay);
        }
    }

    private void SpawnRandomPattern()
    {
        if (coinPrefab == null) return;

        PatternType pattern = GetRandomPattern();

        Vector3 center = new Vector3(
            Random.Range(minX, maxX),
            spawnY,
            0f
        );

        switch (pattern)
        {
            case PatternType.Line:
                SpawnLine(center);
                break;

            case PatternType.ZigZag:
                SpawnZigZag(center);
                break;

            case PatternType.Triangle:
                SpawnTriangle(center);
                break;

            case PatternType.Box:
                SpawnBox(center);
                break;

            case PatternType.Circle:
                SpawnCircle(center);
                break;
        }
    }

    private PatternType GetRandomPattern()
    {
        if (allowedPatterns == null || allowedPatterns.Length == 0)
            return PatternType.Line;

        return allowedPatterns[Random.Range(0, allowedPatterns.Length)];
    }

    private void SpawnLine(Vector3 center)
    {
        for (int i = 0; i < coinsPerPattern; i++)
        {
            SpawnCoin(center + new Vector3(0f, i * spacing, 0f));
        }
    }

    private void SpawnZigZag(Vector3 center)
    {
        for (int i = 0; i < coinsPerPattern; i++)
        {
            float x = i % 2 == 0 ? -spacing : spacing;
            float y = i * spacing;

            SpawnCoin(center + new Vector3(x, y, 0f));
        }
    }

    private void SpawnTriangle(Vector3 center)
    {
        int rows = 3;

        for (int row = 0; row < rows; row++)
        {
            int coinsInRow = row + 1;
            float startX = -(coinsInRow - 1) * spacing * 0.5f;

            for (int col = 0; col < coinsInRow; col++)
            {
                SpawnCoin(center + new Vector3(startX + col * spacing, row * spacing, 0f));
            }
        }
    }

    private void SpawnBox(Vector3 center)
    {
        int width = 4;
        int height = 3;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool edge = y == 0 || y == height - 1 || x == 0 || x == width - 1;
                if (!edge) continue;

                SpawnCoin(center + new Vector3((x - 1.5f) * spacing, y * spacing, 0f));
            }
        }
    }

    private void SpawnCircle(Vector3 center)
    {
        int amount = Mathf.Max(coinsPerPattern, 8);

        for (int i = 0; i < amount; i++)
        {
            float angle = i * Mathf.PI * 2f / amount;

            Vector3 pos = center + new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0f
            );

            SpawnCoin(pos);
        }
    }

    private void SpawnCoin(Vector3 position)
    {
        CoinCollectible coin = Instantiate(coinPrefab, position, Quaternion.identity);
        coin.Initialize(this);
        activeCoins++;
    }

    private void CalculateBounds()
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        minX = cam.transform.position.x - halfWidth + horizontalPadding;
        maxX = cam.transform.position.x + halfWidth - horizontalPadding;
    }
}