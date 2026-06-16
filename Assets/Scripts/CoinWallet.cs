using System;
using UnityEngine;

public sealed class CoinWallet : MonoBehaviour
{
    public static CoinWallet Instance { get; private set; }

    public event Action<int> OnCoinsChanged;

    private const string TotalCoinsKey = "TOTAL_COINS";

    public int TotalCoins { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        TotalCoins = PlayerPrefs.GetInt(TotalCoinsKey, 0);
    }

    private void Start()
    {
        OnCoinsChanged?.Invoke(TotalCoins);
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        TotalCoins += amount;
        Save();

        OnCoinsChanged?.Invoke(TotalCoins);
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0)
            return false;

        if (TotalCoins < amount)
            return false;

        TotalCoins -= amount;
        Save();

        OnCoinsChanged?.Invoke(TotalCoins);
        return true;
    }

    private void Save()
    {
        PlayerPrefs.SetInt(TotalCoinsKey, TotalCoins);
        PlayerPrefs.Save();
    }
}