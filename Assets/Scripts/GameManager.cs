using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
    private UI ui;
    private GameStateManager stateManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ui = FindFirstObjectByType<UI>();
        
        stateManager = GetComponent<GameStateManager>();
        if (stateManager == null)
        {
            stateManager = gameObject.AddComponent<GameStateManager>();
            Debug.Log("GameStateManager agregado automáticamente a GameManager");
        }

        // Inicializar UI
        if (gameOverCanvas != null) gameOverCanvas.SetActive(false);
        if (victoryCanvas != null) victoryCanvas.SetActive(false);

        // Suscribirse a eventos (Observer Pattern)
        // Asegurar que GameEvents existe en la escena; si no, agregarlo a este GameObject
        if (GameEvents.Instance == null)
        {
            var existing = UnityEngine.Object.FindAnyObjectByType<GameEvents>();
            if (existing == null)
            {
                // Añadir GameEvents al GameManager para garantizar la disponibilidad
                gameObject.AddComponent<GameEvents>();
                Debug.Log("GameEvents agregado automáticamente por GameManager");
            }
        }

        // Suscribirse a los eventos (siempre que la instancia exista ahora)
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnGameOver += OnGameOverTriggered;
            GameEvents.Instance.OnVictory += OnVictoryTriggered;
            GameEvents.Instance.OnPlayerDied += OnPlayerDiedTriggered;
        }
        else
        {
            Debug.LogWarning("GameEvents.Instance sigue siendo null; los listeners de GameManager no fueron suscritos.");
        }
    }

    void Update()
    {
        if (stateManager.IsGameActive())
        {
            timer += Time.deltaTime;
        }
    }

    /// <summary>
    /// Maneja el evento de Game Over desde GameEvents (Observer)
    /// </summary>
    private void OnGameOverTriggered()
    {
        stateManager.TransitionToState(stateManager.GameOverState);
        DisplayGameOver();
    }

    /// <summary>
    /// Maneja el evento de Victoria desde GameEvents (Observer)
    /// </summary>
    private void OnVictoryTriggered()
    {
        stateManager.TransitionToState(stateManager.VictoryState);
        DisplayVictory();
    }

    /// <summary>
    /// Maneja el evento de muerte del jugador (Observer)
    /// </summary>
    private void OnPlayerDiedTriggered()
    {
        GameOver();
    }

    /// <summary>
    /// Muestra el canvas de Game Over
    /// </summary>
    private void DisplayGameOver()
    {
        // Pausar el juego
        Time.timeScale = 0f;

        // Notificar a listeners que el juego está pausado
        GameEvents.Instance?.RaiseGamePaused();

        // Mostrar solo la UI de GameOver y ocultar otros Canvas
        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);

        var allCanvases = UnityEngine.Resources.FindObjectsOfTypeAll<UnityEngine.Canvas>();
        foreach (var c in allCanvases)
        {
            if (gameOverCanvas != null)
            {
                if (c.gameObject == gameOverCanvas || c.transform.IsChildOf(gameOverCanvas.transform))
                    continue;
            }
            c.gameObject.SetActive(false);
        }

        if (finalScoreText != null && ui != null) finalScoreText.text = "Score: " + ui.Score;
        if (finalTimeText != null) finalTimeText.text = "Time: " + timer.ToString("F2") + "s";

        // Desactivar input y sistemas de generación / enemigos para 'desaparecer' el juego
        var players = UnityEngine.Resources.FindObjectsOfTypeAll<PlayerController>();
        foreach (var p in players)
        {
            p.enabled = false;
        }

        var spawners = UnityEngine.Resources.FindObjectsOfTypeAll<SpawnerControl>();
        foreach (var s in spawners)
        {
            s.enabled = false;
        }

        var enemies = UnityEngine.Resources.FindObjectsOfTypeAll<Enemy>();
        foreach (var e in enemies)
        {
            e.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Muestra el canvas de Victoria
    /// </summary>
    private void DisplayVictory()
    {
        // Pausar el juego
        Time.timeScale = 0f;

        // Notificar a listeners que el juego está pausado
        GameEvents.Instance?.RaiseGamePaused();

        // Mostrar solo la UI de victoria y ocultar otros Canvas
        if (victoryCanvas != null) victoryCanvas.SetActive(true);

        // Ocultar todos los Canvas de la escena excepto el canvas de victoria
    var allCanvases = UnityEngine.Resources.FindObjectsOfTypeAll<UnityEngine.Canvas>();
        foreach (var c in allCanvases)
        {
            if (victoryCanvas != null)
            {
                // Si el canvas coincide con el objeto de victoryCanvas (o es hijo), lo dejamos activo
                if (c.gameObject == victoryCanvas || c.transform.IsChildOf(victoryCanvas.transform))
                    continue;
            }
            c.gameObject.SetActive(false);
        }

        // Actualizar textos de victoria
        if (victoryScoreText != null && ui != null) victoryScoreText.text = "Score: " + ui.Score;
        if (victoryTimeText != null) victoryTimeText.text = "Time: " + timer.ToString("F2") + "s";

        // Desactivar input y sistemas de generación / enemigos para 'desaparecer' el juego
    var players = UnityEngine.Resources.FindObjectsOfTypeAll<PlayerController>();
        foreach (var p in players)
        {
            p.enabled = false;
        }

    var spawners = UnityEngine.Resources.FindObjectsOfTypeAll<SpawnerControl>();
        foreach (var s in spawners)
        {
            s.enabled = false;
        }

    var enemies = UnityEngine.Resources.FindObjectsOfTypeAll<Enemy>();
        foreach (var e in enemies)
        {
            // Desactivar enemigos para que desaparezcan de la escena
            e.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Llama al sistema de eventos para indicar Game Over
    /// </summary>
    public void GameOver()
    {
        // Ejecutar la transición localmente para mayor robustez
        if (stateManager != null && !(stateManager.GetCurrentState() is GameOverState))
        {
            stateManager.TransitionToState(stateManager.GameOverState);
            DisplayGameOver();
        }

        // Además, notificar a otros listeners si existen
        GameEvents.Instance?.RaiseGameOver();
    }

    /// <summary>
    /// Llama al sistema de eventos para indicar Victoria
    /// </summary>
    public void Victory()
    {
        // Ejecutar la transición localmente para mayor robustez
        if (stateManager != null && !(stateManager.GetCurrentState() is VictoryState))
        {
            stateManager.TransitionToState(stateManager.VictoryState);
            DisplayVictory();
        }

        // Además, notificar a otros listeners si existen
        GameEvents.Instance?.RaiseVictory();
    }

    /// <summary>
    /// Reinicia el nivel actual
    /// </summary>
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Obtiene la puntuación actual
    /// </summary>
    public int GetScore() => ui != null ? ui.Score : 0;

    /// <summary>
    /// Obtiene el tiempo transcurrido
    /// </summary>
    public float GetTime() => timer;

    /// <summary>
    /// Verifica si el juego está activo (state Playing)
    /// </summary>
    public bool IsGameActive() => stateManager.IsGameActive();

    private void OnDestroy()
    {
        // Desuscribirse de eventos para evitar memory leaks
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnGameOver -= OnGameOverTriggered;
            GameEvents.Instance.OnVictory -= OnVictoryTriggered;
            GameEvents.Instance.OnPlayerDied -= OnPlayerDiedTriggered;
        }
    }
}
