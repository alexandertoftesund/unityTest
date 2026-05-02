using UnityEngine;
using System.Collections; // Viktig for å kunne bruke IEnumerator (Coroutines)

public class SpringPad : MonoBehaviour
{
    [Header("Sprett-krefter")]
    [Tooltip("Hvor høyt spilleren spretter opp i luften.")]
    public float upwardForce = 15f;
    [Tooltip("Hvor langt fremover (i spillerens retning) spilleren kastes.")]
    public float forwardForce = 10f;

    [Header("Animasjon (Visuell tyngde)")]
    [Tooltip("Dra inn selve 3D-modellen av puten her. Hvis tom, animeres hele objektet.")]
    public Transform padVisual;
    public float squashAmount = 0.4f; // Hvor flat den blir (0.4 = 40% av original høyde)
    public float squashDuration = 0.05f; // Hvor raskt den trykkes ned (Lynraskt!)
    public float reboundDuration = 0.2f; // Hvor raskt den spretter opp igjen

    [Header("Lyd og effekter (Valgfritt)")]
    public AudioClip springSound;
    [Range(0f, 1f)] public float soundVolume = 0.8f;

    private Vector3 originalScale;
    private Coroutine animationCoroutine;

    void Start()
    {
        // Hvis du ikke har lagt inn en spesifikk grafikk-del, bruker vi selve objektet skriptet ligger på
        if (padVisual == null)
        {
            padVisual = transform;
        }

        // Vi må huske hvor stor puten var originalt, så vi vet hva vi skal sprette tilbake til
        originalScale = padVisual.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 bounceDirection = (transform.up * upwardForce) + (other.transform.forward * forwardForce);
            other.SendMessage("ApplyDirectionalJump", bounceDirection, SendMessageOptions.DontRequireReceiver);

            if (springSound != null)
            {
                AudioSource.PlayClipAtPoint(springSound, transform.position, soundVolume);
            }

            // --- ANIMASJON STARTES HER ---
            // Stopper en eventuell animasjon som allerede kjører (hvis man treffer den to ganger veldig fort)
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
            // Start selve krympe- og sprette-bevegelsen
            animationCoroutine = StartCoroutine(AnimatePad());
        }
    }

    // Dette er funksjonen som animerer puten over tid
    IEnumerator AnimatePad()
    {
        // Vi regner ut målet: Samme X og Z, men lavere Y (trykket sammen)
        Vector3 compressedScale = new Vector3(originalScale.x, originalScale.y * squashAmount, originalScale.z);

        // 1. KLEM NED (Squash)
        float elapsed = 0f;
        while (elapsed < squashDuration)
        {
            elapsed += Time.deltaTime;
            // Lerp blander mykt mellom to størrelser basert på tid
            padVisual.localScale = Vector3.Lerp(originalScale, compressedScale, elapsed / squashDuration);
            yield return null; // Vent til neste frame
        }

        // 2. SPRETT OPP IGJEN (Rebound)
        elapsed = 0f;
        while (elapsed < reboundDuration)
        {
            elapsed += Time.deltaTime;
            padVisual.localScale = Vector3.Lerp(compressedScale, originalScale, elapsed / reboundDuration);
            yield return null;
        }

        // Sørg for at den er 100% tilbake til original størrelse til slutt
        padVisual.localScale = originalScale;
    }
}