using UnityEngine;

public abstract class GameState
{
    protected GameStateManager manager;

    public GameState(GameStateManager stateManager)
    {
        manager = stateManager;
    }

    public virtual void Enter()
    {
        Debug.Log($"Entrando al estado: {GetType().Name}");
    }

    public virtual void Exit()
    {
        Debug.Log($"Saliendo del estado: {GetType().Name}");
    }

    public virtual void Update()
    {
    }

    public abstract void OnStateTransition();
}

public class PlayingState : GameState
{
    public PlayingState(GameStateManager stateManager) : base(stateManager) { }

    public override void Enter()
    {
        base.Enter();
        Time.timeScale = 1f;
        GameEvents.Instance.RaiseGameResumed();
    }

    public override void OnStateTransition()
    {
    }
}

public class PausedState : GameState
{
    public PausedState(GameStateManager stateManager) : base(stateManager) { }

    public override void Enter()
    {
        base.Enter();
        Time.timeScale = 0f;
        GameEvents.Instance.RaiseGamePaused();
    }

    public override void Exit()
    {
        base.Exit();
        Time.timeScale = 1f;
    }

    public override void OnStateTransition()
    {
    }
}

public class GameOverState : GameState
{
    public GameOverState(GameStateManager stateManager) : base(stateManager) { }

    public override void Enter()
    {
        base.Enter();
        Time.timeScale = 0f;
    }

    public override void OnStateTransition()
    {
    }
}

public class VictoryState : GameState
{
    public VictoryState(GameStateManager stateManager) : base(stateManager) { }

    public override void Enter()
    {
        base.Enter();
        Time.timeScale = 0f;
    }

    public override void OnStateTransition()
    {
    }
}
