using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Settings")]
    public CanvasGroup blackScreen;
    public float fadeSpeed = 1f;

    private Transform player;
    private Vector3 spawnPos;
    private Quaternion spawnRot;

    // Awake og Start kjører kun én gang per level nå
    void Awake() => Instance = this;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        if (player) UpdateSpawnPoint(player.position, player.rotation);
        if (blackScreen) StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        for (blackScreen.alpha = 1; blackScreen.alpha > 0; blackScreen.alpha -= fadeSpeed * Time.deltaTime)
            yield return null;
    }

    public void UpdateSpawnPoint(Vector3 pos, Quaternion rot)
    {
        spawnPos = pos;
        spawnRot = rot;
    }

    public void RespawnPlayer()
    {
        var cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false; // Må deaktiveres for å flytte spilleren

        player.SetPositionAndRotation(spawnPos, spawnRot);

        if (cc) cc.enabled = true;
    }

    public void LoadNextLevel()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        // Last neste, eller gå til start (0) hvis vi er på siste brett
        SceneManager.LoadScene(next < SceneManager.sceneCountInBuildSettings ? next : 0);
    }
}