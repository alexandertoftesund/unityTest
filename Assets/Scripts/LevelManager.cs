using UnityEngine;
using UnityEngine.SceneManagement; // Denne må være med for å bytte scener

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private Transform player;
    private Vector3 currentSpawnPoint;

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

    // --- DETTE ER DELEN SOM MANGLER: ---
    public void LoadNextLevel()
    {
        // Vi finner nummeret (index) til banen vi er i nå, og legger til 1
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Vi sjekker om det faktisk finnes en neste scene i Build Settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Ingen flere levels i Build Settings! Går tilbake til start.");
            SceneManager.LoadScene(0); // Laster den aller første scenen (f.eks. menyen)
        }
    }
}