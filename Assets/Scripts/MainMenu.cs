using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scenes")]
    public string level1SceneName = "Level1";

    public void StartGame()
    {
        SceneManager.LoadScene(level1SceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}