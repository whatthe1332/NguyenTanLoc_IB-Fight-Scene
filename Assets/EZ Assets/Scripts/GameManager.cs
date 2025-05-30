using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public enum GameMode { OneVsOne, OneVsMany, ManyVsMany }
    public GameMode currentMode;
    public int currentLevel = 1;
    private int enemyKilledCount = 0;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;
    public GameUIManager gameUIManager;
    public GameObject firstEnemy;
    private List<GameObject> playerTeammates = new List<GameObject>();
    private List<GameObject> enemyTeammates = new List<GameObject>();

    private LevelConfig config;

    void Awake()
    {
        currentLevel = LevelManager.currentLevel;
        Instance = this;
        config = LevelGenerator.GetConfigForLevel(currentLevel);
    }

    void Start()
    {
        enemyKilledCount = 0;
        currentMode = GameModeSelector.SelectedMode;
        CacheTeammates();
        SpawnInitialEnemies();
        if (firstEnemy != null)
            ApplyConfigToEnemy(firstEnemy);
    }

    void CacheTeammates()
    {
        GameObject[] all = GameObject.FindObjectsOfType<GameObject>();
        foreach (var obj in all)
        {
            if (obj.CompareTag("PlayerTeammate"))
                playerTeammates.Add(obj);
            else if (obj.CompareTag("EnemyTeammate"))
                enemyTeammates.Add(obj);
        }
    }

    void SpawnInitialEnemies()
    {
        int spawnCount = config.enemyCount;

        if (currentMode == GameMode.OneVsOne || currentMode == GameMode.ManyVsMany)
            spawnCount = 0;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPos = GetValidNavMeshPosition(enemySpawnPoint.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f)));
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            ApplyConfigToEnemy(enemy);
        }
    }


    void ApplyConfigToEnemy(GameObject enemy)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.speed = config.enemySpeed;

        EnemyAIController ai = enemy.GetComponent<EnemyAIController>();
        if (ai != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                ai.player = player.transform;

            ai.damage = config.enemyDamage;
        }

        HealthSystem health = enemy.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.maxHealth = config.enemyMaxHealth;
            health.currentHealth = health.maxHealth;
            health.UpdateHealthBar();
        }

        EnableNavMesh(enemy);
        EnableHealthBar(enemy);
    }


    public void OnPlayerDead()
    {
        if (playerTeammates.Count > 0)
        {
            GameObject reserve = playerTeammates[0];
            playerTeammates.RemoveAt(0);
            Destroy(reserve);

            Vector3 spawnPos = GetValidNavMeshPosition(playerSpawnPoint.position);
            GameObject newPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

            EnableNavMesh(newPlayer);
            EnableHealthBar(newPlayer);
        }
        else
        {
            gameUIManager.ShowLoseUI();
        }
    }

    public void OnEnemyDead()
    {
        enemyKilledCount++;
        if ((currentMode == GameMode.OneVsOne || currentMode == GameMode.ManyVsMany) && enemyTeammates.Count <= 0)
        {
            if (LevelManager.currentLevel < 10)
                gameUIManager.ShowWinUI();
            else
            {
                gameUIManager.ShowWinModeUI();
                LevelManager.currentLevel = 1;
            }
        }
        if (enemyTeammates.Count > 0)
        {
            GameObject reserve = enemyTeammates[0];
            enemyTeammates.RemoveAt(0);

            #if UNITY_EDITOR
            if (reserve != null && !PrefabUtility.IsPartOfPrefabAsset(reserve))
            {
                Destroy(reserve);
            }
            #else
            if (reserve != null)
            {
                Destroy(reserve);
            }
            #endif

            Vector3 spawnPos = GetValidNavMeshPosition(enemySpawnPoint.position);
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            ApplyConfigToEnemy(newEnemy);
        }
        int totalEnemyToKill = config.enemyCount + 1;
        if (enemyKilledCount >= totalEnemyToKill && currentMode == GameMode.OneVsMany)
        {
            if (LevelManager.currentLevel < 10)
                gameUIManager.ShowWinUI();
            else
            {
                gameUIManager.ShowWinModeUI();
                LevelManager.currentLevel = 1;
            }
        }
    }

    Vector3 GetValidNavMeshPosition(Vector3 originalPosition)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(originalPosition, out hit, 2.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return originalPosition;
    }

    void EnableNavMesh(GameObject obj)
    {
        NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
        if (agent != null && !agent.enabled)
        {
            agent.enabled = true;
        }
    }

    void EnableHealthBar(GameObject obj)
    {
        HealthSystem health = obj.GetComponent<HealthSystem>();
        if (health != null)
        {
            Slider slider = obj.GetComponentInChildren<Slider>(true);
            if (slider != null)
                slider.gameObject.SetActive(true);

            health.currentHealth = health.maxHealth;
            health.UpdateHealthBar();
        }
    }

    public void OnPlayerWin()
    {
        LevelManager.currentLevel++;
        if (currentLevel <= 10)
        {
            gameUIManager.ShowWinUI();
        }
    }
}
