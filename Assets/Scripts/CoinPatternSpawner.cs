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
    [SerializeField] private GameObject coinPrefab;

    [Header("Pattern Settings")]
    [SerializeField] private PatternType[] allowedPatterns;
    [SerializeField] private int coinsPerPattern = 8;
    [SerializeField] private float spacing = 0.45f;
    [SerializeField] private float radius = 1f;

    [Header("Spawn")]
    [SerializeField] private float spawnY = 6f;
    [SerializeField] private float patternSpawnDelay = 1.2f;
    [SerializeField] private float horizontalPadding = 1f;

    private Camera cam;
    private float minX;
    private float maxX;
    private Coroutine routine;

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
            Vector3 pos = center + new Vector3(0f, i * spacing, 0f);
            SpawnCoin(pos);
        }
    }

    private void SpawnZigZag(Vector3 center)
    {
        for (int i = 0; i < coinsPerPattern; i++)
        {
            float x = (i % 2 == 0) ? -spacing : spacing;
            float y = i * spacing;

            SpawnCoin(center + new Vector3(x, y, 0f));
        }
    }

    private void SpawnTriangle(Vector3 center)
    {
        int rows = 4;

        for (int row = 0; row < rows; row++)
        {
            int coinsInRow = row + 1;
            float startX = -(coinsInRow - 1) * spacing * 0.5f;

            for (int col = 0; col < coinsInRow; col++)
            {
                float x = startX + col * spacing;
                float y = row * spacing;

                SpawnCoin(center + new Vector3(x, y, 0f));
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

                Vector3 pos = center + new Vector3(
                    (x - width * 0.5f) * spacing,
                    y * spacing,
                    0f
                );

                SpawnCoin(pos);
            }
        }
    }

    private void SpawnCircle(Vector3 center)
    {
        for (int i = 0; i < coinsPerPattern; i++)
        {
            float angle = i * Mathf.PI * 2f / coinsPerPattern;

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
        Instantiate(coinPrefab, position, Quaternion.identity);
    }

    private void CalculateBounds()
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        minX = cam.transform.position.x - halfWidth + horizontalPadding;
        maxX = cam.transform.position.x + halfWidth - horizontalPadding;
    }
}