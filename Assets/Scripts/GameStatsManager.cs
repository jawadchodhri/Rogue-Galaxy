using System;
using UnityEngine;

public sealed class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance { get; private set; }

    public event Action<int> OnRunCoinsChanged;
    public event Action<int> OnScoreChanged;

    public int RunCoins { get; private set; }
    public int Score { get; private set; }

    private void Awake()
    {
        Instance = this;

        RunCoins = 0;
        Score = 0;
    }

    private void Start()
    {
        OnRunCoinsChanged?.Invoke(RunCoins);
        OnScoreChanged?.Invoke(Score);
    }

    public void AddRunCoin(int amount)
    {
        if (amount <= 0)
            return;

        RunCoins += amount;
        OnRunCoinsChanged?.Invoke(RunCoins);
    }

    public void AddScore(int amount)
    {
        if (amount <= 0)
            return;

        Score += amount;
        OnScoreChanged?.Invoke(Score);
    }
}