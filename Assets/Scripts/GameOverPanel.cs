using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameOverManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalScoreText;
    // [SerializeField] private TMP_Text finalCoinsText;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool gameOver;

    private void Start()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void ShowGameOver()
    {
        if (gameOver == true)
            return;

        gameOver = true;
        Time.timeScale = 0f;

        UpdateGameOverStats();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void UpdateGameOverStats()
    {
        if (GameStatsManager.Instance == null)
            return;

        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + GameStatsManager.Instance.Score.ToString();
        }

        // if (finalCoinsText != null)
        // {
        //     finalCoinsText.text = "Coins: " + GameStatsManager.Instance.Coins.ToString();
        // }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        Debug.Log("Quit only works in build.");
#endif
    }
}