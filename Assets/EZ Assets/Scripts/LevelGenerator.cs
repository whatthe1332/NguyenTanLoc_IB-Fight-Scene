using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static List<LevelConfig> levelConfigs = new List<LevelConfig>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitializeLevels()
    {
        if (levelConfigs.Count == 0)
        {
            for (int i = 1; i <= 10; i++)
            {
                LevelConfig config = new LevelConfig
                {
                    enemyCount = Mathf.Clamp(1 + (i - 1) / 3, 1, 10),
                    enemySpeed = 1f + (i - 1) * 0.2f,
                    enemyDamage = 2 + (i - 1) * 1,
                    enemyMaxHealth = 10 + (i - 1) * 5
                };
                levelConfigs.Add(config);
            }
        }
    }

    public static LevelConfig GetConfigForLevel(int level)
    {
        if (level < 1 || level > levelConfigs.Count)
            level = 1;

        return levelConfigs[level - 1];
    }
}
