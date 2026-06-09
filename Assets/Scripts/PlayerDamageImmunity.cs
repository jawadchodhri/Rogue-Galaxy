using System.Collections;
using UnityEngine;

public sealed class PlayerDamageImmunity : MonoBehaviour
{
    [Header("Immunity")]
    [SerializeField] private float immunityDuration = 1.2f;

    [Header("Opacity Flash")]
    [SerializeField] private float flashInterval = 0.08f;
    [SerializeField] private float visibleAlpha = 1f;
    [SerializeField] private float fadedAlpha = 0.35f;

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    public bool IsImmune
    {
        get { return isImmune; }
    }

    private Color[] originalColors;
    private Coroutine immunityRoutine;
    private bool isImmune;

    private void Awake()
    {
        CacheRenderers();
        CacheOriginalColors();
    }

    private void OnDisable()
    {
        RestoreOriginalColors();
        isImmune = false;
    }

    public void StartImmunity()
    {
        if (immunityRoutine != null)
        {
            StopCoroutine(immunityRoutine);
        }

        immunityRoutine = StartCoroutine(ImmunityRoutine());
    }

    private IEnumerator ImmunityRoutine()
    {
        isImmune = true;

        float timer = 0f;
        float flashTimer = 0f;
        bool faded = false;

        while (timer < immunityDuration)
        {
            timer += Time.deltaTime;
            flashTimer += Time.deltaTime;

            if (flashTimer >= flashInterval)
            {
                flashTimer = 0f;
                faded = !faded;

                if (faded == true)
                {
                    SetAlpha(fadedAlpha);
                }
                else
                {
                    SetAlpha(visibleAlpha);
                }
            }

            yield return null;
        }

        isImmune = false;
        RestoreOriginalColors();
        immunityRoutine = null;
    }

    private void CacheRenderers()
    {
        if (spriteRenderers != null && spriteRenderers.Length > 0)
            return;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void CacheOriginalColors()
    {
        if (spriteRenderers == null)
            return;

        originalColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
                continue;

            originalColors[i] = spriteRenderers[i].color;
        }
    }

    private void SetAlpha(float alpha)
    {
        if (spriteRenderers == null)
            return;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
                continue;

            Color color = spriteRenderers[i].color;
            color.a = alpha;
            spriteRenderers[i].color = color;
        }
    }

    private void RestoreOriginalColors()
    {
        if (spriteRenderers == null)
            return;

        if (originalColors == null)
            return;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
                continue;

            if (i >= originalColors.Length)
                continue;

            spriteRenderers[i].color = originalColors[i];
        }
    }
}