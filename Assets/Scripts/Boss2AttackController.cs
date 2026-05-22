using UnityEngine;

public sealed class Boss2AttackController : MonoBehaviour
{
    private enum AttackType
    {
        PoisonRain,
        DiveDash
    }

    [Header("References")]
    [SerializeField] private BossEntryMovement entryMovement;
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private Boss2PoisonRainAttack poisonRainAttack;
    [SerializeField] private Boss2DiveDashAttack diveDashAttack;

    [Header("Timing")]
    [SerializeField] private float firstAttackDelay = 1f;
    [SerializeField] private float attackDelay = 1.2f;
    [SerializeField] private float phase2AttackDelay = 0.75f;

    [Header("Attack Cycle")]
    [SerializeField] private bool randomizeAttacks = false;

    private readonly AttackType[] attackCycle =
    {
        AttackType.PoisonRain,
        AttackType.DiveDash
    };

    private int attackIndex;

    private float nextAttackTime;

    private bool attackTimerStarted;
    private bool isAttackRunning;

    private void Awake()
    {
        if (entryMovement == null)
        {
            entryMovement = GetComponent<BossEntryMovement>();
        }

        if (bossHealth == null)
        {
            bossHealth = GetComponent<BossHealth>();
        }

        if (poisonRainAttack == null)
        {
            poisonRainAttack = GetComponent<Boss2PoisonRainAttack>();
        }

        if (diveDashAttack == null)
        {
            diveDashAttack = GetComponent<Boss2DiveDashAttack>();
        }
    }

    private void Update()
    {
        if (CanAttack() == false)
            return;

        if (attackTimerStarted == false)
        {
            attackTimerStarted = true;
            nextAttackTime = Time.time + firstAttackDelay;
            return;
        }

        if (isAttackRunning == true)
            return;

        if (Time.time < nextAttackTime)
            return;

        StartNextAttack();
    }

    private bool CanAttack()
    {
        if (entryMovement != null)
        {
            if (entryMovement.HasEntered == false)
                return false;
        }

        return true;
    }

    private void StartNextAttack()
    {
        AttackType attackType = GetNextAttack();

        isAttackRunning = true;

        if (attackType == AttackType.PoisonRain)
        {
            if (poisonRainAttack != null)
            {
                poisonRainAttack.BeginAttack(HandleAttackCompleted);
                return;
            }
        }

        if (attackType == AttackType.DiveDash)
        {
            if (diveDashAttack != null)
            {
                diveDashAttack.BeginAttack(HandleAttackCompleted);
                return;
            }
        }

        HandleAttackCompleted();
    }

    private AttackType GetNextAttack()
    {
        if (randomizeAttacks == true)
        {
            int randomIndex = Random.Range(0, attackCycle.Length);
            return attackCycle[randomIndex];
        }

        AttackType selectedAttack = attackCycle[attackIndex];

        attackIndex++;

        if (attackIndex >= attackCycle.Length)
        {
            attackIndex = 0;
        }

        return selectedAttack;
    }

    private void HandleAttackCompleted()
    {
        isAttackRunning = false;
        SetNextAttackDelay();
    }

    private void SetNextAttackDelay()
    {
        float delay;

        if (IsPhase2() == true)
        {
            delay = phase2AttackDelay;
        }
        else
        {
            delay = attackDelay;
        }

        nextAttackTime = Time.time + delay;
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