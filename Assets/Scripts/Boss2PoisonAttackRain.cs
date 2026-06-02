using System;
using System.Collections;
using UnityEngine;

public sealed class Boss2PoisonRainAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Boss2LaneMovement laneMovement;
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private EnemyProjectile poisonProjectilePrefab;

    [Header("Spawn Area")]
    [SerializeField] private float horizontalPadding = 0.5f;
    [SerializeField] private float spawnTopPadding = 1f;

    [Header("Phase 1")]
    [SerializeField] private float attackDuration = 2f;
    [SerializeField] private float spawnInterval = 0.25f;
    [SerializeField] private float dropsPerWave = 1;

    [Header("Phase 2")]
    [SerializeField] private float phase2AttackDuration = 2.4f;
    [SerializeField] private float phase2SpawnInterval = 0.16f;
    [SerializeField] private float phase2DropsPerWave = 2;

    private Camera mainCamera;
    private Coroutine attackRoutine;

    private float minX;
    private float maxX;
    private float spawnY;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (laneMovement == null)
        {
            laneMovement = GetComponent<Boss2LaneMovement>();
        }

        if (bossHealth == null)
        {
            bossHealth = GetComponent<BossHealth>();
        }

        CalculateSpawnArea();
    }

    public void BeginAttack(Action onComplete)
    {
        if (attackRoutine != null)
            return;

        attackRoutine = StartCoroutine(AttackRoutine(onComplete));
    }

    private IEnumerator AttackRoutine(Action onComplete)
    {
        if (laneMovement != null)
        {
            laneMovement.PauseMovement();
        }

        float duration = GetAttackDuration();
        float interval = GetSpawnInterval();
        float currentDropsPerWave = GetDropsPerWave();

        float endTime = Time.time + duration;
        float nextSpawnTime = Time.time;

        while (Time.time < endTime)
        {
            if (Time.time >= nextSpawnTime)
            {
                SpawnPoisonWave((int)currentDropsPerWave);
                nextSpawnTime = Time.time + interval;
            }

            yield return null;
        }

        if (laneMovement != null)
        {
            laneMovement.ResumeMovement();
        }

        attackRoutine = null;

        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }

    private void SpawnPoisonWave(int count)
    {
        if (poisonProjectilePrefab == null)
            return;

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = new Vector3(
                UnityEngine.Random.Range(minX, maxX),
                spawnY,
                0f
            );

            EnemyProjectile projectile = Instantiate(
                poisonProjectilePrefab,
                spawnPosition,
                Quaternion.identity
            );

            projectile.Initialize(Vector2.down);
        }
    }

    private void CalculateSpawnArea()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Boss2PoisonRainAttack: Main Camera not found.");
            return;
        }

        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = halfHeight * mainCamera.aspect;

        minX = mainCamera.transform.position.x - halfWidth + horizontalPadding;
        maxX = mainCamera.transform.position.x + halfWidth - horizontalPadding;
        spawnY = mainCamera.transform.position.y + halfHeight + spawnTopPadding;
    }

    private float GetAttackDuration()
    {
        if (IsPhase2() == true)
            return phase2AttackDuration;

        return attackDuration;
    }

    private float GetSpawnInterval()
    {
        if (IsPhase2() == true)
            return phase2SpawnInterval;

        return spawnInterval;
    }

    private float GetDropsPerWave()
    {
        if (IsPhase2() == true)
            return phase2DropsPerWave;

        return dropsPerWave;
    }

    private bool IsPhase2()
    {
        if (bossHealth == null)
            return false;

        if (bossHealth.IsPhase2 == true)
            return true;

        return false;
    }
}