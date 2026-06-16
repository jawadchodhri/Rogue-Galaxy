using TMPro;
using UnityEngine;

public sealed class TopHUDUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text runCoinText;
    [SerializeField] private TMP_Text scoreText;

    private void OnEnable()
    {
        if (GameStatsManager.Instance == null)
            return;

        GameStatsManager.Instance.OnRunCoinsChanged += UpdateRunCoins;
        GameStatsManager.Instance.OnScoreChanged += UpdateScore;

        UpdateRunCoins(GameStatsManager.Instance.RunCoins);
        UpdateScore(GameStatsManager.Instance.Score);
    }

    private void OnDisable()
    {
        if (GameStatsManager.Instance == null)
            return;

        GameStatsManager.Instance.OnRunCoinsChanged -= UpdateRunCoins;
        GameStatsManager.Instance.OnScoreChanged -= UpdateScore;
    }

    private void UpdateRunCoins(int coins)
    {
        if (runCoinText != null)
        {
            runCoinText.text = coins.ToString();
        }
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }
}