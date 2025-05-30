using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static void LoadLevel(GameManager.GameMode mode, int level)
    {
        string sceneName = GetSceneName(mode, level);
        LevelManager.currentLevel = level;
        SceneManager.LoadScene(sceneName);
    }

    private static string GetSceneName(GameManager.GameMode mode, int level)
    {
        switch (mode)
        {
            case GameManager.GameMode.OneVsOne:
                return $"1v1_lv{level}";
            case GameManager.GameMode.OneVsMany:
                return $"1vM_lv{level}";
            case GameManager.GameMode.ManyVsMany:
                return $"MvM_lv{level}";
            default:
                Debug.LogError("Unknown mode: " + mode);
                return "MainMenu";
        }
    }
}
