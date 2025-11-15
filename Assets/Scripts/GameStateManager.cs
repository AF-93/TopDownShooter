using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private GameState currentState;
    private bool isInitialized = false;

    private PlayingState playingState;
    private PausedState pausedState;
    private GameOverState gameOverState;
    private VictoryState victoryState;

    public PlayingState PlayingState
    {
        get
        {
            if (!isInitialized) InitializeStates();
            return playingState;
        }
    }

    public PausedState PausedState
    {
        get
        {
            if (!isInitialized) InitializeStates();
            return pausedState;
        }
    }

    public GameOverState GameOverState
    {
        get
        {
            if (!isInitialized) InitializeStates();
            return gameOverState;
        }
    }

    public VictoryState VictoryState
    {
        get
        {
            if (!isInitialized) InitializeStates();
            return victoryState;
        }
    }

    private void Awake()
    {
        InitializeStates();
        TransitionToState(PlayingState);
    }

    private void InitializeStates()
    {
        if (isInitialized) return;

        if (GameEvents.Instance == null)
        {
            Debug.LogError("GameEvents.Instance es null. Asegúrate de que GameEvents está en la escena.");
            return;
        }

        playingState = new PlayingState(this);
        pausedState = new PausedState(this);
        gameOverState = new GameOverState(this);
        victoryState = new VictoryState(this);

        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;
        currentState?.Update();
    }

    public void TransitionToState(GameState newState)
    {
        if (!isInitialized) InitializeStates();

        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public GameState GetCurrentState()
    {
        if (!isInitialized) InitializeStates();
        return currentState;
    }

    public bool IsGameActive()
    {
        if (!isInitialized) return true;
        return currentState is PlayingState;
    }
}
