using System.Collections.Generic;

public class UndoStack
{
    private Stack<GameState> undoes;
    private bool undoing;
    private GameState startState;

    public UndoStack()
    {
        undoes = new Stack<GameState>();
        undoing = false;
        startState = GameState.Make();
    }

    public void Undo()
    {
        if (undoes.Count > 0)
        {
            if (undoes.Count > 1 && !undoing)
            {
                undoes.Pop();
                undoing = true;
            }

            var state = undoes.Pop();
            state.Apply();
        }
        else
        {
            startState.Apply();
        }
    }

    public void Do()
    {
        undoing = false;
        undoes.Push(GameState.Make());
    }
}