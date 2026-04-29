using UnityEngine;

public class EnemyBottomRespawn : MonoBehaviour
{
    [SerializeField] private float bottomPadding = 1f;

    private EnemyWaveMember waveMember;
    private Camera mainCamera;

    private void Awake()
    {
        waveMember = GetComponent<EnemyWaveMember>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        float bottomY = mainCamera.transform.position.y - mainCamera.orthographicSize - bottomPadding;

        if (transform.position.y < bottomY)
        {
            waveMember.Die();
        }
    }
}
