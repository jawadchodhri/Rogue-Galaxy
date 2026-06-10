using TMPro;
using UnityEngine;

public sealed class TopHUDUI : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text scoreText;

    //[Header("Labels")]
    //[SerializeField] private string coinPrefix = "";
    //[SerializeField] private string scorePrefix = "";

    private void OnEnable()
    {
        if (GameStatsManager.Instance == null)
            return;

        GameStatsManager.Instance.OnCoinsChanged += UpdateCoins;
        GameStatsManager.Instance.OnScoreChanged += UpdateScore;

        UpdateCoins(GameStatsManager.Instance.Coins);
        UpdateScore(GameStatsManager.Instance.Score);
    }

    private void OnDisable()
    {
        if (GameStatsManager.Instance == null)
            return;

        GameStatsManager.Instance.OnCoinsChanged -= UpdateCoins;
        GameStatsManager.Instance.OnScoreChanged -= UpdateScore;
    }

    private void UpdateCoins(int coins)
    {
        if (coinText == null)
            return;

        coinText.text = coins.ToString();
    }

    private void UpdateScore(int score)
    {
        if (scoreText == null)
            return;

        scoreText.text = score.ToString();
    }
}