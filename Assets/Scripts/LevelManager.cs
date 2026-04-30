using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Viktig for Coroutines

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Global UI")]
    public CanvasGroup blackScreen;
    public float globalFadeSpeed = 1.0f; // Hvor fort vi fader ut i starten av et nivå

    private Transform player;
    private Vector3 currentSpawnPoint;
    private Quaternion currentSpawnRotation;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayerInScene();

        // Start "Fade In" hver gang et nytt nivå lastes
        if (blackScreen != null)
        {
            StartCoroutine(FadeInSequence());
        }
    }

    // --- NY COROUTINE FOR Å FADE INN NÅR NIVÅET STARTER ---
    IEnumerator FadeInSequence()
    {
        // Vi sørger for at skjermen er helt svart først
        blackScreen.alpha = 1.0f;

        // Vent en ørliten brøkdel så alt er lastet ferdig
        yield return new WaitForSeconds(0.1f);

        while (blackScreen.alpha > 0.0f)
        {
            blackScreen.alpha -= globalFadeSpeed * Time.deltaTime;
            yield return null;
        }
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
    }

    public void UpdateSpawnPoint(Vector3 newPosition, Quaternion newRotation)
    {
        currentSpawnPoint = newPosition;
        currentSpawnRotation = newRotation;
    }

    public void RespawnPlayer()
    {
        if (player == null) FindPlayerInScene();

        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.position = currentSpawnPoint;
            player.rotation = currentSpawnRotation;

            if (cc != null) cc.enabled = true;
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