using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Dette gjør at andre script lett kan få tak i manageren
    public static LevelManager Instance { get; private set; }

    private Transform player;
    private Vector3 currentSpawnPoint;

    private void Awake()
    {
        // Sjekker om det allerede finnes en LevelManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Dette gjør at objektet ikke slettes når du bytter scene
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Vi lytter etter når en scene er ferdig lastet
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Finn spilleren automatisk i den nye scenen
        FindPlayerInScene();
    }

    private void FindPlayerInScene()
    {
        GameObject foundPlayer = GameObject.FindWithTag("Player");
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
            // Setter første spawnpoint til der spilleren starter i den nye banen
            currentSpawnPoint = player.position;
        }
    }

    public void UpdateSpawnPoint(Vector3 newPosition)
    {
        currentSpawnPoint = newPosition;
    }

    public void RespawnPlayer()
    {
        if (player == null) FindPlayerInScene();

        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.position = currentSpawnPoint;

            if (cc != null) cc.enabled = true;
        }
    }
}