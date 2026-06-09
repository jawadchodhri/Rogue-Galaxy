using UnityEngine;

public sealed class HitImpactVFXPool : MonoBehaviour
{
    public static HitImpactVFXPool Instance { get; private set; }

    [Header("Pool")]
    [SerializeField] private HitImpactVFX hitVFXPrefab;
    [SerializeField] private int poolSize = 20;

    private HitImpactVFX[] pool;
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
        if (hitVFXPrefab == null)
        {
            Debug.LogError("HitImpactVFXPool: Hit VFX prefab is missing.");
            return;
        }

        pool = new HitImpactVFX[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = CreateNewVFX();
        }
    }

    private HitImpactVFX CreateNewVFX()
    {
        HitImpactVFX vfx = Instantiate(hitVFXPrefab, transform);
        vfx.gameObject.SetActive(false);
        return vfx;
    }

    public void Play(Vector3 position, Transform followTarget)
    {
        if (pool == null)
            return;

        if (pool.Length == 0)
            return;

        HitImpactVFX vfx = pool[nextIndex];

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

        vfx.Play(position, followTarget);
    }
}