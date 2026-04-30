using UnityEngine;

public class WallPusher : MonoBehaviour
{
    public Vector3 direction = Vector3.forward;
    public float distance = 2f;
    public float speed = 3f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool goingOut = true;

    private void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + direction.normalized * distance;
    }

    private void Update()
    {
        Vector3 target = goingOut ? endPosition : startPosition;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            goingOut = !goingOut;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CharacterController controller = other.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.Move(direction.normalized * speed * Time.deltaTime);
        }
    }
}