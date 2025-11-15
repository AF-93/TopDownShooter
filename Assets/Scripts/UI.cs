using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    private int score = 0;
    public int Score => score;
    private float timer = 0f;
    private bool isGameActive = true;

    void Start()
    {
        UpdateScore(0);
        UpdateTimer(0f);

        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnScoreAdded += OnScoreAdded;
            GameEvents.Instance.OnGameOver += OnGameOver;
            GameEvents.Instance.OnVictory += OnVictory;
            GameEvents.Instance.OnGamePaused += OnGamePaused;
            GameEvents.Instance.OnGameResumed += OnGameResumed;
        }
    }

    void Update()
    {
        if (isGameActive)
        {
            timer += Time.deltaTime;
            UpdateTimer(timer);
        }
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScore(score);
        
        GameEvents.Instance?.RaiseScoreAdded(value);

        if (score >= GameManager.Instance.targetScore)
        {
            GameManager.Instance.Victory();
        }
    }

    private void UpdateScore(int value)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + value;
    }

    private void UpdateTimer(float time)
    {
        if (timerText != null)
            timerText.text = "Time: " + time.ToString("F2") + "s";
    }

    private void OnScoreAdded(int amount)
    {
        Debug.Log($"Puntuación añadida: {amount}");
    }

    private void OnGameOver()
    {
        StopTimer();
    }

    private void OnVictory()
    {
        StopTimer();
    }

    private void OnGamePaused()
    {
        isGameActive = false;
    }

    private void OnGameResumed()
    {
        isGameActive = true;
    }

    public void StopTimer()
    {
        isGameActive = false;
    }

    private void OnDestroy()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnScoreAdded -= OnScoreAdded;
            GameEvents.Instance.OnGameOver -= OnGameOver;
            GameEvents.Instance.OnVictory -= OnVictory;
            GameEvents.Instance.OnGamePaused -= OnGamePaused;
            GameEvents.Instance.OnGameResumed -= OnGameResumed;
        }
    }
}
