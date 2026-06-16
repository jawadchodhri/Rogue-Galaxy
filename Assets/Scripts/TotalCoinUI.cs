using TMPro;
using UnityEngine;

public sealed class TotalCoinUI : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;

    private void OnEnable()
    {
        if (CoinWallet.Instance == null)
            return;

        CoinWallet.Instance.OnCoinsChanged += UpdateCoins;
        UpdateCoins(CoinWallet.Instance.TotalCoins);
    }

    private void OnDisable()
    {
        if (CoinWallet.Instance == null)
            return;

        CoinWallet.Instance.OnCoinsChanged -= UpdateCoins;
    }

    private void UpdateCoins(int coins)
    {
        if (coinText != null)
        {
            coinText.text = coins.ToString();
        }
    }
}