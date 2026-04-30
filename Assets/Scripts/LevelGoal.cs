using UnityEngine;
using System.Collections;

public class LevelGoal : MonoBehaviour
{
    [Header("Visuelt")]
    public float rotationSpeed = 50f;
    public GameObject collectEffect;

    [Header("Overgang")]
    public float victoryDelay = 1.0f;
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
        // 1. GJEM DIAMANTEN UMIDDELBART
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // Vi skrur av både utseende og kollisjon på diamanten med en gang
        if (GetComponent<Renderer>() != null) GetComponent<Renderer>().enabled = false;
        if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;

        CharacterController cc = player.GetComponent<CharacterController>();
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        Animator anim = player.GetComponent<Animator>();

        // 2. VENT PÅ AT SPILLEREN LANDER
        // Vi venter med å skru av 'movement' slik at tyngdekraften fortsatt fungerer
        float safetyTimer = 2.0f;
        if (cc != null)
        {
            while (!cc.isGrounded && safetyTimer > 0)
            {
                safetyTimer -= Time.deltaTime;
                yield return null;
            }
        }

        // 3. NÅ HAR VI LANDET - STOPP KONTROLL OG SPILL ANIMASJON
        if (movement != null) movement.enabled = false;
        if (anim != null) anim.SetTrigger("Win");

        // 4. SEIERS-PAUSE (Her kan spilleren se animasjonen sin)
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