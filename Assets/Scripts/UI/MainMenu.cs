using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scenes")]
    public string level1SceneName = "Level1";

    [Header("UI Panels")]
    public GameObject levelSelectPanel;

    private void Start()
    {
        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(false);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(level1SceneName);
    }

    public void OpenLevelSelect()
    {
        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(true);
        }
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene(level1SceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}