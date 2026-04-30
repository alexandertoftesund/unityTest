using UnityEngine;
using System.Collections;

public class RespawnHandler : MonoBehaviour
{
    [Header("Innstillinger")]
    public float fadeSpeed = 1.0f;
    public float floatHeight = 4.0f;
    public float floatDownSpeed = 2.0f;
    public float landingDelay = 0.3f;

    private bool isRespawning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isRespawning && other.CompareTag("Player") && LevelManager.Instance.blackScreen != null)
        {
            StartCoroutine(RespawnSequence(other.gameObject));
        }
    }

    IEnumerator RespawnSequence(GameObject player)
    {
        isRespawning = true;
        CanvasGroup uiFade = LevelManager.Instance.blackScreen;
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        CharacterController cc = player.GetComponent<CharacterController>();
        Vector3 originalScale = player.transform.localScale;

        // 1. STOPP OG SKJUL
        if (movement != null) movement.enabled = false;
        if (cc != null) cc.enabled = false;

        float elapsed = 0;
        while (elapsed < 0.4f)
        {
            player.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsed / 0.4f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 2. FADE TIL SVART
        while (uiFade.alpha < 1.0f)
        {
            uiFade.alpha += fadeSpeed * Time.deltaTime;
            yield return null;
        }

        // 3. TELEPORTERING OG SKALERING
        player.transform.localScale = originalScale;
        LevelManager.Instance.RespawnPlayer();

        if (cc != null) cc.enabled = false;

        Vector3 targetGroundPos = player.transform.position;
        player.transform.position = targetGroundPos + Vector3.up * floatHeight;

        yield return new WaitForSeconds(0.2f);

        // 4. FADE UT
        while (uiFade.alpha > 0.0f)
        {
            uiFade.alpha -= fadeSpeed * Time.deltaTime;
            yield return null;
        }

        // 5. FLYTE NED
        while (Vector3.Distance(player.transform.position, targetGroundPos) > 0.05f)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetGroundPos, floatDownSpeed * Time.deltaTime);
            yield return null;
        }

        // 6. LANDING
        player.transform.position = targetGroundPos;
        yield return new WaitForSeconds(landingDelay);

        // 7. AKTIVER KONTROLL
        if (cc != null) cc.enabled = true;
        if (movement != null) movement.enabled = true;

        isRespawning = false;
    }
}