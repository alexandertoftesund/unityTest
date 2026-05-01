using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Settings")]
    public CanvasGroup blackScreen;

    [Header("Respawn Innstillinger")]
    public float fadeSpeed = 1f;
    public float floatHeight = 5.0f;
    public float floatDownSpeed = 4.0f;
    public float landingDelay = 0.25f;

    [Header("Lyd")]
    public AudioSource audioSource;
    public AudioClip shrinkSound;
    [Range(0, 1)] public float soundVolume = 0.5f;

    private Transform player;
    private Vector3 spawnPos;
    private Quaternion spawnRot;
    private bool isRespawning = false;

    void Awake()
    {
        Instance = this;
        // Sikrer at vi har en høyttaler
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        if (player) UpdateSpawnPoint(player.position, player.rotation);
        if (blackScreen) StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        if (blackScreen)
        {
            blackScreen.alpha = 1;
            while (blackScreen.alpha > 0)
            {
                blackScreen.alpha -= fadeSpeed * Time.deltaTime;
                yield return null;
            }
        }
    }

    public void UpdateSpawnPoint(Vector3 pos, Quaternion rot)
    {
        spawnPos = pos;
        spawnRot = rot;
    }

    public void StartRespawn(GameObject playerObj)
    {
        if (!isRespawning)
        {
            StartCoroutine(RespawnSequence(playerObj));
        }
    }

    private IEnumerator RespawnSequence(GameObject playerObj)
    {
        isRespawning = true;

        PlayerMovement movement = playerObj.GetComponent<PlayerMovement>();
        CharacterController cc = playerObj.GetComponent<CharacterController>();
        Vector3 originalScale = playerObj.transform.localScale;

        // Skru av kontroll
        if (movement != null) movement.enabled = false;
        if (cc != null) cc.enabled = false;

        // --- SPILL KRYMPE-LYDEN HER ---
        if (audioSource != null && shrinkSound != null)
        {
            audioSource.PlayOneShot(shrinkSound, soundVolume);
        }

        // 1. KRYMP (Spilleren forsvinner visuelt der han døde)
        float elapsed = 0;
        while (elapsed < 0.4f)
        {
            playerObj.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsed / 0.4f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 2. FADE TIL SVART
        if (blackScreen != null)
        {
            while (blackScreen.alpha < 1.0f)
            {
                blackScreen.alpha += fadeSpeed * Time.deltaTime;
                yield return null;
            }
        }

        // --- RESETTER FIENDER ---
        Enemy[] allEnemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy e in allEnemies) e.ResetEnemy();

        // 3. TELEPORTERING (Mens skjermen er svart)
        playerObj.transform.localScale = originalScale;
        if (cc) cc.enabled = false;
        playerObj.transform.SetPositionAndRotation(spawnPos, spawnRot);

        // Klargjør fall-posisjon
        Vector3 targetGroundPos = playerObj.transform.position;
        playerObj.transform.position = targetGroundPos + Vector3.up * floatHeight;

        // 4. BEVEGELSE NED OG FADE UT (Samtidig - ingen venting!)
        while (Vector3.Distance(playerObj.transform.position, targetGroundPos) > 0.05f || (blackScreen != null && blackScreen.alpha > 0))
        {
            // Beveg karakteren nedover
            playerObj.transform.position = Vector3.MoveTowards(playerObj.transform.position, targetGroundPos, floatDownSpeed * Time.deltaTime);

            // Fade ut skjermen
            if (blackScreen != null && blackScreen.alpha > 0)
            {
                blackScreen.alpha -= fadeSpeed * Time.deltaTime;
            }

            yield return null;
        }

        // 5. FERDIG LANDET
        playerObj.transform.position = targetGroundPos;

        yield return new WaitForSeconds(landingDelay);

        // Gi kontrollen tilbake
        if (cc != null) cc.enabled = true;
        if (movement != null) movement.enabled = true;

        isRespawning = false;
    }

    public void LoadNextLevel()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(next < SceneManager.sceneCountInBuildSettings ? next : 0);
    }
}