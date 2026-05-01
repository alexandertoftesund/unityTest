using UnityEngine;
using System.Collections;

public class LevelGoal : MonoBehaviour
{
    [Header("Visuelt")]
    public float rotationSpeed = 50f;
    public GameObject collectEffect;

    [Header("Lyd")]
    public AudioClip victorySound;
    private AudioSource audioSource;

    [Header("Overgang")]
    public float startDelay = 0.2f;
    public float victoryDelay = 4.0f;
    public float fadeSpeed = 1.0f;

    private bool hasReachedGoal = false;

    void Awake()
    {
        // ENDRET: Nå leter vi også i barn-objekter etter Audio Source
        audioSource = GetComponentInChildren<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogWarning("Fant ingen AudioSource på " + gameObject.name + " eller dens barn!");
        }
    }

    void Update()
    {
        if (!hasReachedGoal)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasReachedGoal)
        {
            hasReachedGoal = true;
            StartCoroutine(FinishLevelSequence(other.gameObject));
        }
    }

    IEnumerator FinishLevelSequence(GameObject player)
    {
        yield return new WaitForSeconds(startDelay);

        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        if (GetComponent<Renderer>() != null) GetComponent<Renderer>().enabled = false;
        if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;

        CharacterController cc = player.GetComponent<CharacterController>();
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        Animator anim = player.GetComponentInChildren<Animator>();

        // Vent til spilleren har landet
        float safetyTimer = 2.0f;
        if (cc != null)
        {
            while (!cc.isGrounded && safetyTimer > 0)
            {
                safetyTimer -= Time.deltaTime;
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.3f);

        // --- SEIERSSEKVENS ---
        if (movement != null) movement.enabled = false;

        // Spiller lyden hvis vi har funnet komponenten og en fil
        if (audioSource != null && victorySound != null)
        {
            audioSource.PlayOneShot(victorySound);
        }

        if (anim != null)
        {
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.y = player.transform.position.y;
            player.transform.LookAt(cameraPos);

            // Starter jubel-animasjonen fra Animator-vinduet
            anim.CrossFade("Cheering", 0.2f);
        }

        yield return new WaitForSeconds(victoryDelay);

        // Tones ut til svart
        CanvasGroup uiFade = LevelManager.Instance.blackScreen;
        if (uiFade != null)
        {
            while (uiFade.alpha < 1.0f)
            {
                uiFade.alpha += fadeSpeed * Time.deltaTime;
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.5f);
        LevelManager.Instance.LoadNextLevel();
    }
}