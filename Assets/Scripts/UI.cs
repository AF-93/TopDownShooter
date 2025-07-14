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

    public void StopTimer()
    {
        isGameActive = false;
    }
}
