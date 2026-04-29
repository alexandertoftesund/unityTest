using UnityEngine;

public class CannonTrigger : MonoBehaviour
{
    public Cannon cannonScript;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Starter skytingen når spilleren går inn i sonen
        if (other.CompareTag("Player") && !hasTriggered)
        {
            cannonScript.StartFiring();
            hasTriggered = true; // Sørger for at vi ikke starter loopen flere ganger
        }
    }

    // VALGFRITT: Hvis du vil at kanonen skal stoppe når spilleren går ut av sonen
    /*
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cannonScript.StopFiring();
            hasTriggered = false;
        }
    }
    */
}