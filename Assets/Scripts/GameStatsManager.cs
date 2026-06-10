using System;
using UnityEngine;

public sealed class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance { get; private set; }

    public event Action<int> OnCoinsChanged;
    public event Action<int> OnScoreChanged;

    [Header("Starting Values")]
    [SerializeField] private int startingCoins;
    [SerializeField] private int startingScore;

    public int Coins { get; private set; }
    public int Score { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Coins = startingCoins;
        Score = startingScore;
    }

    private void Start()
    {
        OnCoinsChanged?.Invoke(Coins);
        OnScoreChanged?.Invoke(Score);
    }

    public void AddCoin(int amount)
    {
        if (amount <= 0)
            return;

        Coins += amount;
        OnCoinsChanged?.Invoke(Coins);
    }

    public void AddScore(int amount)
    {
        if (amount <= 0)
            return;

        Score += amount;
        OnScoreChanged?.Invoke(Score);
    }

    public void ResetStats()
    {
        Coins = startingCoins;
        Score = startingScore;

        OnCoinsChanged?.Invoke(Coins);
        OnScoreChanged?.Invoke(Score);
    }
}