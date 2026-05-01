using UnityEngine;

public class Cannon : MonoBehaviour
{
    public GameObject bullet;
    public Transform cannon;
    public float fireRate = 2.0f;

    [Header("Lyd")]
    public AudioSource audioSource;
    public AudioClip fireSound;
    [Range(0, 1)] public float fireVolume = 0.5f;

    private bool hasTriggered = false;

    void Start()
    {
        // Forsøker å finne AudioSource automatisk hvis den ikke er lagt inn
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            InvokeRepeating("Fire", 0f, fireRate);
        }
    }

    void Fire()
    {
        if (bullet != null && cannon != null)
        {
            Instantiate(bullet, cannon.position, cannon.rotation);

            // Spill av lyden
            if (audioSource != null && fireSound != null)
            {
                // Legger til litt tilfeldig pitch for mer naturlig kanonlyd
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(fireSound, fireVolume);
            }
        }
    }

    public void StopFiring()
    {
        CancelInvoke("Fire");
    }
}