using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private int unlocked;
    private bool startNewGame;
    private Text newGame, continueGame;
    private Color active, inactive;

    private void Start()
    {
        startNewGame = true;

        newGame = transform.FindChild("New Game").GetComponent<Text>();
        continueGame = transform.FindChild("Continue").GetComponent<Text>();

        active = newGame.color;
        inactive = continueGame.color;

        unlocked = Settings.UnlockedLevel;
    }

    private void Update()
    {
        var toggled = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow);

        if (unlocked > 0 && toggled)
        {
            startNewGame = !startNewGame;

            if (startNewGame)
            {
                newGame.color = active;
                continueGame.color = inactive;
            }
            else
            {
                newGame.color = inactive;
                continueGame.color = active;
            }
        }

        var confirmed = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);

        if (confirmed)
        {
            if (startNewGame)
            {
                Settings.UnlockedLevel = 0;
                Transitions.FadeToScene(0.3f, 1);
            }
            else
            {
                Transitions.FadeToScene(0.3f, unlocked);
            }
        }
    }
}
