using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Global UI")]
    public CanvasGroup blackScreen;
    public float globalFadeSpeed = 1.0f;

    private Transform player;
    private Vector3 currentSpawnPoint;
    private Quaternion currentSpawnRotation;

    private void Awake()
    {
        // Hver scene har sin egen LevelManager.
        // Ingen DontDestroyOnLoad, så level 1 og level 2 bruker hver sin riktige UI/spawn.
        Instance = this;
    }

    private void Start()
    {
        FindPlayerInScene();

        if (blackScreen != null)
        {
            StartCoroutine(FadeInSequence());
        }
        else
        {
            Debug.LogWarning("LevelManager: BlackScreen er ikke koblet i Inspector.");
        }
    }

    private IEnumerator FadeInSequence()
    {
        blackScreen.alpha = 1.0f;

        yield return new WaitForSeconds(0.1f);

        while (blackScreen.alpha > 0.0f)
        {
            blackScreen.alpha -= globalFadeSpeed * Time.deltaTime;
            yield return null;
        }

        blackScreen.alpha = 0.0f;
    }

    private void FindPlayerInScene()
    {
        GameObject foundPlayer = GameObject.FindWithTag("Player");

        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
            currentSpawnPoint = player.position;
            currentSpawnRotation = player.rotation;
        }
        else
        {
            Debug.LogWarning("LevelManager: Fant ingen GameObject med tag 'Player'.");
        }
    }

    public void UpdateSpawnPoint(Vector3 newPosition, Quaternion newRotation)
    {
        currentSpawnPoint = newPosition;
        currentSpawnRotation = newRotation;
    }

    public void RespawnPlayer()
    {
        if (player == null)
        {
            FindPlayerInScene();
        }

        if (player == null)
        {
            Debug.LogWarning("LevelManager: Kan ikke respawne fordi Player mangler.");
            return;
        }

        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;
        }

        player.position = currentSpawnPoint;
        player.rotation = currentSpawnRotation;

        if (cc != null)
        {
            cc.enabled = true;
        }
    }

    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}