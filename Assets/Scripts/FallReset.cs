using UnityEngine;

public class FallReset : MonoBehaviour
{
    [Header("Referanse til Spawn")]
    // Her drar du inn objektet spilleren skal lande på (SpawnPad)
    public Transform spawnPad;

    private void OnTriggerEnter(Collider other)
    {
        // Sjekker om det er spilleren som traff sonen
        if (other.CompareTag("Player"))
        {
            // Vi henter CharacterControlleren
            CharacterController controller = other.GetComponent<CharacterController>();

            if (controller != null && spawnPad != null)
            {
                // VIKTIG: Vi må skru av controlleren før vi flytter den
                controller.enabled = false;

                // Flytt spilleren til SpawnPad sin posisjon
                other.transform.position = spawnPad.position;

                // Skru den på igjen etterpå
                controller.enabled = true;

                Debug.Log("Spilleren falt utfor og ble sendt til spawn!");
            }
        }
    }
}