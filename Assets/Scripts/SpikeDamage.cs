using UnityEngine;
using UnityEngine.SceneManagement; // Denne trengs for å starte på nytt

public class SpikeDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            Debug.Log("Spilleren døde!");

            // ALTERNATIV 1: Slett spilleren fra spillet (Instadød)
            Destroy(other.gameObject);

            // ALTERNATIV 2: Start hele brettet på nytt (nyttig for testing)
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}