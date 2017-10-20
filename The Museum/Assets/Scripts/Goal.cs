using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class for the goal object. Triggers the transition to the next level.
/// </summary>
public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Movement>();

        if (player != null)
        {
            var currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            var nextLevelIndex = currentLevelIndex + 1;


            if (nextLevelIndex >= SceneManager.sceneCountInBuildSettings)
            {
                Transitions.FadeToScene(0.3f, 0);
            }
            else
            {
                Transitions.FadeToScene(0.3f, nextLevelIndex);
            }
        }
    }

    private void OnLevelWasLoaded()
    {
        var currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

        if (Settings.UnlockedLevel < currentLevelIndex)
        {
            Settings.UnlockedLevel = currentLevelIndex;
        }
    }
}
