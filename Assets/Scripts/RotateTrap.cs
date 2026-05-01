using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RotateTrap : MonoBehaviour
{
    [Header("Bevegelse")]
    public float rotationSpeed = 180f; // Grader per sekund
    public bool randomizeRotation = true;
    public bool addSpeedVariation = true;

    [Header("Lyd")]
    public AudioSource audioSource;
    public AudioClip rotationSound;
    [Range(0, 1)] public float volume = 0.5f;

    [Header("Synkronisering")]
    [Tooltip("Dersom lyden skal vare nøyaktig én rotasjon, huk av her.")]
    public bool syncPitchToRotation = true;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // 1. Legg til litt tilfeldig fart hvis ønskelig
        if (addSpeedVariation)
        {
            rotationSpeed += Random.Range(-15f, 15f);
        }

        // 2. Start-rotasjon
        if (randomizeRotation)
        {
            transform.Rotate(0f, Random.Range(0f, 360f), 0f);
        }

        if (audioSource != null && rotationSound != null)
        {
            audioSource.clip = rotationSound;
            audioSource.volume = volume;
            audioSource.loop = true;

            if (syncPitchToRotation)
            {
                // MATEMATIKKEN:
                // En hel runde er 360 grader.
                // Tiden det tar for én runde = 360 / rotationSpeed.
                float timePerRotation = 360f / Mathf.Abs(rotationSpeed);

                // For at lyden skal matche, må pitch være (Lydens lengde / Tiden det tar å rotere)
                // Hvis lyden er 2 sek og rotasjonen tar 1 sek, må den spilles dobbelt så fort (pitch 2).
                audioSource.pitch = rotationSound.length / timePerRotation;
            }
            else
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
            }

            // 3. Start på tilfeldig sted i lydfilen så de ikke starter i takt
            audioSource.time = Random.Range(0f, rotationSound.length);
            audioSource.Play();
        }
    }

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}