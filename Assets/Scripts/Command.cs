using UnityEngine;

public interface ICommand
{
    void Execute();
    void Undo();
}

public class FireCommand : ICommand
{
    private PlayerController playerController;

    public FireCommand(PlayerController controller)
    {
        playerController = controller;
    }

    public void Execute()
    {
        if (playerController != null)
        {
            playerController.ExecuteFire();
        }
    }

    public void Undo()
    {
    }
}

public class BurstFireCommand : ICommand
{
    private PlayerController playerController;

    public BurstFireCommand(PlayerController controller)
    {
        playerController = controller;
    }

    public void Execute()
    {
        if (playerController != null)
        {
            playerController.ExecuteBurstFire();
        }
    }

    public void Undo()
    {
    }
}

public class MoveCommand : ICommand
{
    private PlayerController playerController;
    private Vector2 direction;

    public MoveCommand(PlayerController controller, Vector2 moveDirection)
    {
        playerController = controller;
        direction = moveDirection;
    }

    public void Execute()
    {
        if (playerController != null)
        {
            playerController.ExecuteMove(direction);
        }
    }

    public void Undo()
    {
        if (playerController != null)
        {
            playerController.ExecuteMove(Vector2.zero);
        }
    }
}

public class LookCommand : ICommand
{
    private PlayerController playerController;
    private Vector2 mousePosition;

    public LookCommand(PlayerController controller, Vector2 position)
    {
        playerController = controller;
        mousePosition = position;
    }

    public void Execute()
    {
        if (playerController != null)
        {
            playerController.ExecuteLook(mousePosition);
        }
    }

    public void Undo()
    {
    }
}
