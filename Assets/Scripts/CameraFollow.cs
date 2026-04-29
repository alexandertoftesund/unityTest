using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Mål")]
    public Transform target;
    private CharacterController targetController;

    [Header("Vinkel-innstillinger")]
    public float height = 9.0f;
    public float distance = 4.0f;

    [Header("Smoothing (Senket for stabilitet)")]
    // Hvor raskt kameraet flytter seg fysisk (Posisjon)
    public float positionSmooth = 4f;
    // Hvor raskt kameraet svinger rundt spilleren (Orbit) - SENKET
    public float orbitSmooth = 1.2f;
    // Hvor raskt selve blikket justerer seg (Look) - SENKET
    public float lookSmooth = 1.5f;

    private float lastGroundedY;

    private void Start()
    {
        if (target != null)
        {
            targetController = target.GetComponent<CharacterController>();
            lastGroundedY = target.position.y;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 1. BEREGN ROTASJON (Veldig treg svinging)
        float angle = transform.eulerAngles.y;
        float targetAngle = target.eulerAngles.y;

        // Mathf.LerpAngle med lav orbitSmooth gjør at kameraet "lagger" behagelig etter
        float lerpedAngle = Mathf.LerpAngle(angle, targetAngle, orbitSmooth * Time.deltaTime);
        Quaternion rotation = Quaternion.Euler(0, lerpedAngle, 0);

        // 2. OPPDATER BAKKE-HØYDE
        if (targetController != null && targetController.isGrounded)
        {
            lastGroundedY = Mathf.Lerp(lastGroundedY, target.position.y, 4f * Time.deltaTime);
        }

        // 3. BEREGN POSISJON
        Vector3 flatPosition = target.position - (rotation * Vector3.forward * distance);
        Vector3 wantedPosition = new Vector3(flatPosition.x, lastGroundedY + height, flatPosition.z);

        transform.position = Vector3.Lerp(transform.position, wantedPosition, positionSmooth * Time.deltaTime);

        // 4. SE PÅ SPILLEREN (Mykt fokus)
        // Vi ser på et punkt foran spilleren, men med lav lookSmooth for å unngå rykk
        Vector3 lookAtPoint = new Vector3(target.position.x, lastGroundedY, target.position.z) + (target.forward * 2f);
        Quaternion targetRotation = Quaternion.LookRotation(lookAtPoint - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookSmooth * Time.deltaTime);
    }
}