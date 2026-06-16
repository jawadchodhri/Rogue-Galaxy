using TMPro;
using UnityEngine;

public sealed class RespawnOfferManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameOverManager gameOverManager;

    [Header("UI")]
    [SerializeField] private GameObject respawnPanel;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text totalCoinsText;
    [SerializeField] private TMP_Text respawnCostText;

    [Header("Respawn Settings")]
    [SerializeField] private float countdownDuration = 5f;
    [SerializeField] private int startingRespawnCost = 10;

    private PlayerHealth currentPlayer;
    private float countdownTimer;

    private int currentRespawnCost;
    private bool isShowing;

    private void Awake()
    {
        currentRespawnCost = startingRespawnCost;

        if (gameOverManager == null)
        {
            gameOverManager = FindAnyObjectByType<GameOverManager>();
        }

        if (respawnPanel != null)
        {
            respawnPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (isShowing == false)
            return;

        countdownTimer -= Time.unscaledDeltaTime;

        UpdateUI();

        if (countdownTimer > 0f)
            return;

        ShowGameOver();
    }

    public void ShowRespawnOffer(PlayerHealth playerHealth)
    {
        if (isShowing == true)
            return;

        currentPlayer = playerHealth;
        countdownTimer = countdownDuration;
        isShowing = true;

        Time.timeScale = 0f;

        if (respawnPanel != null)
        {
            respawnPanel.SetActive(true);
        }

        UpdateUI();
    }

    public void RespawnButtonPressed()
    {
        if (isShowing == false)
        return;

        if (CoinWallet.Instance == null)
            return;

        if (CoinWallet.Instance.SpendCoins(currentRespawnCost) == false)
            return;

        if (currentPlayer != null)
        {
            currentPlayer.RespawnFromDeath();
        }

        currentRespawnCost *= 2;

        CloseRespawnPanel();

        Time.timeScale = 1f;
    }

    public void SkipRespawnButtonPressed()
    {
        if (isShowing == false)
            return;

        ShowGameOver();
    }

    private void ShowGameOver()
    {
        CloseRespawnPanel();

        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }
    }

    private void CloseRespawnPanel()
    {
        isShowing = false;
        currentPlayer = null;

        if (respawnPanel != null)
        {
            respawnPanel.SetActive(false);
        }
    }

    private void UpdateUI()
    {
        int shownTime = Mathf.CeilToInt(countdownTimer);

        if (countdownText != null)
        {
            countdownText.text = shownTime.ToString();
        }

        if (totalCoinsText != null)
        {
            int coins = 0;

            if (CoinWallet.Instance != null)
            {
                coins = CoinWallet.Instance.TotalCoins;
            }

            totalCoinsText.text = coins.ToString();
        }

        if (respawnCostText != null)
        {
            respawnCostText.text = currentRespawnCost.ToString();
        }
    }
}