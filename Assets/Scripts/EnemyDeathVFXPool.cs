using UnityEngine;

public sealed class EnemyDeathVFXPool : MonoBehaviour
{
    public static EnemyDeathVFXPool Instance { get; private set; }

    [Header("Pool")]
    [SerializeField] private EnemyDeathVFX deathVFXPrefab;
    [SerializeField] private int poolSize = 15;

    private EnemyDeathVFX[] pool;
    private int nextIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CreatePool();
    }

    private void CreatePool()
    {
        if (deathVFXPrefab == null)
        {
            Debug.LogError("EnemyDeathVFXPool: Death VFX prefab is missing.");
            return;
        }

        pool = new EnemyDeathVFX[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = CreateNewVFX();
        }
    }

    private EnemyDeathVFX CreateNewVFX()
    {
        EnemyDeathVFX vfx = Instantiate(deathVFXPrefab, transform);
        vfx.gameObject.SetActive(false);
        return vfx;
    }

    public void Play(Vector3 position)
    {
        if (pool == null || pool.Length == 0)
            return;

        EnemyDeathVFX vfx = pool[nextIndex];

        if (vfx == null)
        {
            vfx = CreateNewVFX();
            pool[nextIndex] = vfx;
        }

        nextIndex++;

        if (nextIndex >= pool.Length)
        {
            nextIndex = 0;
        }

        vfx.Play(position);
    }
}