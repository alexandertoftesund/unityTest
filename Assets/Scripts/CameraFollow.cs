using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Mål")]
    public Transform target;
    private CharacterController targetController;

    [Header("Innstillinger")]
    public float height = 5.0f; // Justert ned litt for huler
    public float distance = 4.0f;
    public float positionSmooth = 4f;
    public float orbitSmooth = 1.2f;
    public float lookSmooth = 1.5f;

    [Header("Kollisjon")]
    public LayerMask collisionLayers; // Velg hvilke lag som er vegger
    public float collisionPadding = 0.5f; // Litt avstand fra veggen

    private float lastGroundedY;

    void Start()
    {
        if (target == null) target = GameObject.FindWithTag("Player")?.transform;

        if (target)
        {
            targetController = target.GetComponent<CharacterController>();
            lastGroundedY = target.position.y;
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        // 1. ROTASJON
        float lerpedAngle = Mathf.LerpAngle(transform.eulerAngles.y, target.eulerAngles.y, orbitSmooth * Time.deltaTime);
        Quaternion rotation = Quaternion.Euler(0, lerpedAngle, 0);

        // 2. BAKKE-HØYDE
        if (targetController?.isGrounded ?? false)
            lastGroundedY = Mathf.Lerp(lastGroundedY, target.position.y, 4f * Time.deltaTime);

        // 3. POSISJON (Beregner først hvor kameraet VIL være)
        Vector3 wantedPos = target.position - (rotation * Vector3.forward * distance);
        wantedPos.y = lastGroundedY + height;

        // --- NYTT: Sjekk for kollisjon mellom spiller og ønsket posisjon ---
        Vector3 rayOrigin = target.position + Vector3.up * 1.5f; // Skyt fra spillerens "hode"
        Vector3 direction = wantedPos - rayOrigin;

        if (Physics.Raycast(rayOrigin, direction.normalized, out RaycastHit hit, direction.magnitude, collisionLayers))
        {
            // Vi traff noe! Flytt kameraet til treffpunktet minus litt padding
            wantedPos = hit.point - direction.normalized * collisionPadding;
        }

        transform.position = Vector3.Lerp(transform.position, wantedPos, positionSmooth * Time.deltaTime);

        // 4. SE PÅ SPILLEREN
        Vector3 lookAtPoint = new Vector3(target.position.x, lastGroundedY + 1.5f, target.position.z) + (target.forward * 2f);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookAtPoint - transform.position), lookSmooth * Time.deltaTime);
    }
}