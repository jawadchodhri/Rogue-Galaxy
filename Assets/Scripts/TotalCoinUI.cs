using System.Collections;
using TMPro;
using UnityEngine;

public sealed class TotalCoinUI : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private string prefix = "";

    private Coroutine waitRoutine;

    private void OnEnable()
    {
        waitRoutine = StartCoroutine(WaitForWalletRoutine());
    }

    private void OnDisable()
    {
        if (waitRoutine != null)
        {
            StopCoroutine(waitRoutine);
            waitRoutine = null;
        }

        if (CoinWallet.Instance != null)
        {
            CoinWallet.Instance.OnCoinsChanged -= UpdateCoins;
        }
    }

    private IEnumerator WaitForWalletRoutine()
    {
        while (CoinWallet.Instance == null)
        {
            yield return null;
        }

        CoinWallet.Instance.OnCoinsChanged += UpdateCoins;
        UpdateCoins(CoinWallet.Instance.TotalCoins);
    }

    private void UpdateCoins(int coins)
    {
        if (coinText == null)
            return;

        coinText.text = prefix + coins.ToString();
    }
}