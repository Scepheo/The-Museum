using UnityEngine;

/// <summary>
/// Persistent game data
/// </summary>
public static class Settings
{
    private const string unlockedLevelKey = "UnlockedLevel";

    /// <summary>
    /// The highest unlocked level
    /// </summary>
    public static int UnlockedLevel
    {
        get { return PlayerPrefs.GetInt(unlockedLevelKey, 0); }
        set { PlayerPrefs.SetInt(unlockedLevelKey, value); }
    }
}
