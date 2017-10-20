using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Transitions
{
    // Ensures only one transition can run at a time
    private static bool running;

    /// <summary>
    /// Quits the application, fading to black.
    /// </summary>
    /// <param name="time">The duration of the fade</param>
    public static void FadeExit(float time)
    {
        FadeExit(time, Color.black);
    }

    /// <summary>
    /// Quits the application, fading to a given color.
    /// </summary>
    /// <param name="time">The duration of the fade</param>
    /// <param name="color">Color to fade to</param>
    public static void FadeExit(float time, Color color)
    {
        if (!running)
        {
            createFadeObject(time, () => Application.Quit(), color);
            running = true;
        }
    }

    /// <summary>
    /// Reloads the current scene, with a fade to black.
    /// </summary>
    /// <param name="time">The duration of the fade</param>
    public static void FadeRestart(float time)
    {
        FadeRestart(time, Color.black);
    }

    /// <summary>
    /// Reloads the current scene, with a fade to a given color.
    /// </summary>
    /// <param name="time">The duration of the fade</param>
    /// <param name="color">Color to fade to</param>
    public static void FadeRestart(float time, Color color)
    {
        var currentScene = SceneManager.GetActiveScene();
        FadeToScene(time, currentScene.name, color);
    }

    /// <summary>
    /// Loads a given scene, with a fade to black.
    /// </summary>
    /// <param name="time">The duration of the transition</param>
    /// <param name="scene">The scene to load</param>
    public static void FadeToScene(float time, string scene)
    {
        FadeToScene(time, scene, Color.black);
    }

    /// <summary>
    /// Loads a given scene, with a fade to black.
    /// </summary>
    /// <param name="time">The duration of the transition</param>
    /// <param name="scene">The scene to load</param>
    public static void FadeToScene(float time, int scene)
    {
        FadeToScene(time, scene, Color.black);
    }

    /// <summary>
    /// Loads a given scene, with a fade to a given color.
    /// </summary>
    /// <param name="time">The duration of the transition</param>
    /// <param name="scene">The scene to load</param>
    /// <param name="color">Color to fade to</param>
    public static void FadeToScene(float time, string scene, Color color)
    {
        if (!running)
        {
            createFadeObject(time / 2f, () => SceneManager.LoadScene(scene), color);
            running = true;
        }
    }

    /// <summary>
    /// Loads a given scene, with a fade to a given color.
    /// </summary>
    /// <param name="time">The duration of the transition</param>
    /// <param name="scene">The scene to load</param>
    /// <param name="color">Color to fade to</param>
    public static void FadeToScene(float time, int scene, Color color)
    {
        if (!running)
        {
            createFadeObject(time / 2f, () => SceneManager.LoadScene(scene), color);
            running = true;
        }
    }

    // Helper function to create a fade object
    private static void createFadeObject(float time, Action action, Color color)
    {
        var obj = new GameObject("FadeObject");
        var scr = obj.AddComponent<FadeScript>();
        scr.SetValues(time, action, color);
    }

    // Behaviour class for fader object
    private class FadeScript : MonoBehaviour
    {
        // Default values
        private float fadeTime = 1f;
        private Action action = (() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        private Color fadeColor = Color.black;

        private float startTime;
        private bool fadeOut;
        private RawImage gui;

        private void Start()
        {
            // Freeze game
            Time.timeScale = 0f;
            startTime = Time.realtimeSinceStartup;
            fadeOut = true;

            // Make object persistent
            DontDestroyOnLoad(gameObject);

            // Create and configure gui texture
            gui = gameObject.AddComponent<RawImage>();
            gui.uvRect = new Rect(0f, 0f, 1f, 1f);
            gui.color = Color.clear;

            // Add a canvas
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
        }

        public void SetValues(float fadeTime, Action action, Color color)
        {
            this.fadeTime = fadeTime;
            this.action = action;
            this.fadeColor = color;
        }

        private void OnLevelWasLoaded()
        {
            fadeOut = false;
            startTime = Time.realtimeSinceStartup;
        }

        private void Update()
        {
            var time = Time.realtimeSinceStartup - startTime;

            if (fadeOut)
            {
                if (time < fadeTime)
                {
                    gui.color = Color.Lerp(Color.clear, fadeColor, time / fadeTime);
                }
                else
                {
                    action();
                }
            }
            else
            {
                if (time < fadeTime)
                {
                    gui.color = Color.Lerp(fadeColor, Color.clear, time / fadeTime);
                }
                else
                {
                    Time.timeScale = 1f;
                    Transitions.running = false;
                    GameObject.Destroy(this.gameObject);
                }
            }
        }
    }
}