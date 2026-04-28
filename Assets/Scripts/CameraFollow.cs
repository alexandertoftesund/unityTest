using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Karakteren kameraet skal følge
    public Transform target;

    // Hvor smooth kameraet følger etter
    public float followSpeed = 8f;

    // Avstanden mellom kameraet og karakteren
    private Vector3 offset;

    private void Start()
    {
        // Hvis vi ikke har satt target i Inspector, stopper vi her
        if (target == null)
        {
            Debug.LogWarning("CameraFollow mangler target!");
            return;
        }

        // Lagrer avstanden kameraet har til karakteren når spillet starter
        // Eksempel: hvis kameraet står bak og over karakteren,
        // holder det seg bak og over karakteren
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        // Hvis target mangler, gjør ingenting
        if (target == null)
        {
            return;
        }

        // Finner hvor kameraet burde være
        Vector3 wantedPosition = target.position + offset;

        // Flytter kameraet smooth mot ønsket posisjon
        transform.position = Vector3.Lerp(
            transform.position,
            wantedPosition,
            followSpeed * Time.deltaTime
        );
    }
}