using System.Collections.Generic;

public class UndoStack
{
    private Stack<GameState> gameStates;
    private GameState startState;

    public UndoStack()
    {
        gameStates = new Stack<GameState>();
        startState = GameState.Make();
        gameStates.Push(startState);
    }

    /// <summary>
    /// Pops the last gamestate and applies it.
    /// </summary>
    public void Undo()
    {
        if (gameStates.Count > 0)
        {
            var state = gameStates.Pop();
            state.Apply();
        }
    }

    /// <summary>
    /// Pushes the current gamestate onto the stack. Should be called before any movement.
    /// </summary>
    public void Do()
    {
        var newGameState = GameState.Make();
        gameStates.Push(newGameState);
    }
}