using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string menuSceneName = "MainMenu";

    public string activeSceneName = "Level 1_Manager";

    public string[] level1Scenes =
    {
        "Level1_Environment",
        "Level1_Gameplay",
        "Level1_Manager"
    };

    public void StartGame()
    {
        StartCoroutine(LoadLevel1());
    }

    private IEnumerator LoadLevel1()
    {
        foreach (string sceneName in level1Scenes)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                while (!loadOperation.isDone)
                {
                    yield return null;
                }
            }
        }

        Scene activeScene = SceneManager.GetSceneByName(activeSceneName);

        if (activeScene.IsValid() && activeScene.isLoaded)
        {
            SceneManager.SetActiveScene(activeScene);
        }
        else
        {
            Debug.LogWarning("Could not set active scene: " + activeSceneName);
        }

        Scene menuScene = SceneManager.GetSceneByName(menuSceneName);

        if (menuScene.IsValid() && menuScene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(menuScene);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}