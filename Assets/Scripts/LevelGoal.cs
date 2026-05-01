using UnityEngine;
using System.Collections;

public class LevelGoal : MonoBehaviour
{
    [Header("Visuelt")]
    public float rotationSpeed = 50f;
    public GameObject collectEffect;

    [Header("Overgang")]
    public float startDelay = 0.2f;
    public float victoryDelay = 4.0f; // <-- ENDRET: Her styrer du hvor lenge han jubler
    public float fadeSpeed = 1.0f;

    private bool hasReachedGoal = false;

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
        // 1. START-DELAY OG EFFEKT
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

        // 2. VENT PÅ AT SPILLEREN LANDER
        float safetyTimer = 2.0f;
        if (cc != null)
        {
            while (!cc.isGrounded && safetyTimer > 0)
            {
                safetyTimer -= Time.deltaTime;
                yield return null;
            }
        }

        // En bitteliten pause etter landing før han snur seg og jubler
        yield return new WaitForSeconds(0.3f);

        // 3. STOPP KONTROLL OG START JUBEL
        if (movement != null) movement.enabled = false;

        if (anim != null)
        {
            // Snur karakteren mot kameraet
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.y = player.transform.position.y;
            player.transform.LookAt(cameraPos);

            // Spiller "Cheering" fra bildet ditt
            anim.CrossFade("Cheering", 0.2f);
        }

        // 4. DEN LANGE JUBLE-PAUSEN
        // Her blir karakteren stående i "Cheering"-staten helt til tiden er ute
        yield return new WaitForSeconds(victoryDelay);

        // 5. FADE TIL SVART
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