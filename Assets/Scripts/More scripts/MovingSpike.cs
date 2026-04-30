using UnityEngine;

public class MovingHazard : MonoBehaviour
{
    [Header("Movement")]
    public float moveDistance = 2f;
    public float moveSpeed = 2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Sin gir en myk opp/ned-bevegelse mellom -1 og 1
        float movement = Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        transform.position = startPosition + Vector3.up * movement;
    }
}