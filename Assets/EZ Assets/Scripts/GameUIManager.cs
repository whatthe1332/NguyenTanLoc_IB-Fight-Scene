using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject winmodePanel;
    public GameObject pauseButton;
    public GameObject pauseGrid;
    public Text levelText;

    void Awake()
    {
        Instance = this;
        winPanel.SetActive(false);
        winmodePanel.SetActive(false);
        losePanel.SetActive(false);
        pauseButton.SetActive(true);
        pauseGrid.SetActive(false);
    }
    void Start()
    {
        UpdateLevelText(); 
    }

    public void UpdateLevelText()
    {
        if (levelText != null)
            levelText.text = "Level " + LevelManager.currentLevel;
    }

    public void ShowWinUI()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowLoseUI()
    {
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ShowWinModeUI()
    {
        winmodePanel.SetActive(true);
        Time.timeScale = 0f;
    }


    public void OnNextLevelButton()
    {
        Time.timeScale = 1f;
        GameManager gm = GameManager.Instance;
        if (gm != null)
        {
            LevelManager.currentLevel++;
            SceneLoader.LoadLevel(gm.currentMode, LevelManager.currentLevel); 
        }
        else
        {
            Debug.LogError("GameManager not found!");
        }
    }

    public void OnTryAgainButton()
    {
        Time.timeScale = 1f;
        GameManager gm = GameManager.Instance;
        if (gm != null)
        {
            SceneLoader.LoadLevel(gm.currentMode, gm.currentLevel); 
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        if (LevelManager.currentLevel > PlayerPrefs.GetInt(GameManager.Instance.currentMode + "_HighLevel", 0))
            PlayerPrefs.SetInt(GameManager.Instance.currentMode + "_HighLevel", LevelManager.currentLevel);
        LevelManager.currentLevel = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void onPauseButton()
    {
        Time.timeScale = 0f;
        pauseGrid.SetActive(true);
    }

    public void onResumeButton()
    {
        Time.timeScale = 1f;
        pauseGrid.SetActive(false);
    }
    public void onExitButton()
    {
        Time.timeScale = 1f;
        pauseGrid.SetActive(false);
        if (LevelManager.currentLevel > PlayerPrefs.GetInt(GameManager.Instance.currentMode + "_HighLevel", 0))
            PlayerPrefs.SetInt(GameManager.Instance.currentMode + "_HighLevel", LevelManager.currentLevel);
        LevelManager.currentLevel = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
