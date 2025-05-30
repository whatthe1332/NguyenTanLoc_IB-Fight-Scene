using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public GameObject titleText;
    public GameObject startButton;
    public GameObject modePanel;
    public Text highestlvl1v1text;
    public Text highestlvl1vMtext;
    public Text highestlvlMvMtext;
    void Start()
    {
        titleText.SetActive(true);
        startButton.SetActive(true);
        modePanel.SetActive(false);
        if (PlayerPrefs.GetInt("OneVsOne_HighLevel").ToString() != "")
            highestlvl1v1text.text = "HighestLevel: " + PlayerPrefs.GetInt("OneVsOne_HighLevel", 0).ToString();
        if (PlayerPrefs.GetInt("OneVsOne_HighLevel").ToString() != "")
            highestlvl1vMtext.text = "HighestLevel: " + PlayerPrefs.GetInt("OneVsMany_HighLevel", 0).ToString();
        if (PlayerPrefs.GetInt("OneVsOne_HighLevel").ToString() != "")
            highestlvlMvMtext.text = "HighestLevel: " + PlayerPrefs.GetInt("ManyVsMany_HighLevel", 0).ToString();
        Time.timeScale = 0f;
    }

    public void OnStartButtonPressed()
    {
        startButton.SetActive(false);
        modePanel.SetActive(true);
    }

    public void SelectMode(int mode)
    {
        GameModeSelector.SelectedMode = (GameManager.GameMode)mode;
        int highestlevel = 1;
        if (GameModeSelector.SelectedMode.ToString() == "OneVsOne")
            highestlevel = PlayerPrefs.GetInt("OneVsOne_HighLevel", 1);
        if (GameModeSelector.SelectedMode.ToString() == "OneVsMany")
            highestlevel = PlayerPrefs.GetInt("OneVsMany_HighLevel", 1);
        if (GameModeSelector.SelectedMode.ToString() == "ManyVsMany")
            highestlevel = PlayerPrefs.GetInt("ManyVsMany_HighLevel", 1);
        Time.timeScale = 1f;
        SceneLoader.LoadLevel(GameModeSelector.SelectedMode, highestlevel);
    }

    public void SelectTest()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("testlag");
    }
}
