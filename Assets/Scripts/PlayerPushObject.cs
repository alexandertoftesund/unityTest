using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    // Hvor hardt spilleren dytter objekter
    public float pushPower = 3f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Bare objekter med taggen "Pushable" kan dyttes
        if (!hit.gameObject.CompareTag("Pushable"))
        {
            return;
        }

        // Hent Rigidbody fra objektet vi traff
        Rigidbody rb = hit.gameObject.GetComponent<Rigidbody>();

        // Hvis objektet ikke har Rigidbody, gjør ingenting
        if (rb == null)
        {
            return;
        }

        // Dytt bare bortover langs bakken, ikke oppover. Viktig!
        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);

        // Dytt objektet
        rb.AddForce(pushDirection * pushPower, ForceMode.Impulse);
    }
}