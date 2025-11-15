using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public static GameEvents Instance { get; private set; }

    public event Action<int> OnScoreAdded;
    public event Action OnPlayerDied;
    public event Action OnEnemyDied;
    public event Action OnVictory;
    public event Action OnGameOver;
    public event Action OnGamePaused;
    public event Action OnGameResumed;
    public event Action<int> OnPlayerHealthChanged;
    public event Action<int> OnEnemySpawned;
    public event Action<int> OnEnemyDespawned;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RaiseScoreAdded(int amount)
    {
        OnScoreAdded?.Invoke(amount);
    }

    public void RaisePlayerDied()
    {
        OnPlayerDied?.Invoke();
    }

    public void RaiseEnemyDied()
    {
        OnEnemyDied?.Invoke();
    }

    public void RaiseVictory()
    {
        OnVictory?.Invoke();
    }

    public void RaiseGameOver()
    {
        OnGameOver?.Invoke();
    }

    public void RaiseGamePaused()
    {
        OnGamePaused?.Invoke();
    }

    public void RaiseGameResumed()
    {
        OnGameResumed?.Invoke();
    }

    public void RaisePlayerHealthChanged(int currentHealth)
    {
        OnPlayerHealthChanged?.Invoke(currentHealth);
    }

    public void RaiseEnemySpawned(int totalEnemies)
    {
        OnEnemySpawned?.Invoke(totalEnemies);
    }

    public void RaiseEnemyDespawned(int totalEnemies)
    {
        OnEnemyDespawned?.Invoke(totalEnemies);
    }
}
