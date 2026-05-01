using UnityEngine;

public class RespawnHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Vi sjekker kun om det er spilleren, og ber LevelManager starte sekvensen
        if (other.CompareTag("Player") && LevelManager.Instance != null)
        {
            LevelManager.Instance.StartRespawn(other.gameObject);
        }
    }
}