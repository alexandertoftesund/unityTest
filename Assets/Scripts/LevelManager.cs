using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Global UI")]
    public CanvasGroup blackScreen;

    private Transform player;
    private Vector3 currentSpawnPoint;
    private Quaternion currentSpawnRotation; // Ny variabel for retning

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
    }

    private void FindPlayerInScene()
    {
        GameObject foundPlayer = GameObject.FindWithTag("Player");
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
            currentSpawnPoint = player.position;
            currentSpawnRotation = player.rotation; // Lagrer retningen spilleren har ved start
        }
    }

    // Oppdatert for å også kunne ta imot ny retning (f.eks. fra en checkpoint)
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
            player.rotation = currentSpawnRotation; // Setter retningen tilbake til start

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