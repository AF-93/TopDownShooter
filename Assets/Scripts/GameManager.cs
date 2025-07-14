using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public GameObject gameOverCanvas;
    public GameObject victoryCanvas;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI victoryScoreText;
    public TextMeshProUGUI victoryTimeText;


    [Header("Gameplay")]
    public int targetScore = 1000;

    private float timer = 0f;
    private bool isGameActive = true;
    private UI ui;

    void Awake()
    {   
        ui = FindFirstObjectByType<UI>();
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        Time.timeScale = 1f;
        if (gameOverCanvas != null) gameOverCanvas.SetActive(false);
        if (victoryCanvas != null) victoryCanvas.SetActive(false);
    }

    void Update()
    {
        if (isGameActive)
        {
            timer += Time.deltaTime;
        }
    }

    public void GameOver()
    {
        isGameActive = false;
        Time.timeScale = 0f;
        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
        if (finalScoreText != null && ui != null) finalScoreText.text = "Score: " + ui.Score;
        if (finalTimeText != null) finalTimeText.text = "Time: " + timer.ToString("F2") + "s";
    }

    public void Victory()
    {
        isGameActive = false;
        Time.timeScale = 0f;
        if (victoryCanvas != null) victoryCanvas.SetActive(true);
        if (victoryScoreText != null && ui != null) victoryScoreText.text = "Score: " + ui.Score;
        if (victoryTimeText != null) victoryTimeText.text = "Time: " + timer.ToString("F2") + "s";
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public int GetScore() { return ui != null ? ui.Score : 0; }
    public float GetTime() { return timer; }
    public bool IsGameActive() { return isGameActive; }
}
