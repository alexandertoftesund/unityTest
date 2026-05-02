using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu";

    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject gameGuidePanel;
    public GameObject settingsPanel;

    private bool isPaused = false;

    private void Start()
    {
        pauseMenuPanel.SetActive(false);

        if (gameGuidePanel != null)
            gameGuidePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Time.timeScale = 1f;
        LockCursor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ContinueGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        UnlockCursor();
    }

    public void ContinueGame()
    {
        pauseMenuPanel.SetActive(false);

        if (gameGuidePanel != null)
            gameGuidePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        LockCursor();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OpenGameGuide()
    {
        if (gameGuidePanel != null)
            gameGuidePanel.SetActive(true);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}