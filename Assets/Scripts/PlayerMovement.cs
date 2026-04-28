using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float gravity = -20f;
    public float jumpHeight = 2f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Sjekker om vi står på bakken
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Holder oss klistret til bakken
        }

        // Henter input (WASD)
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // Bevegelse basert på kameraet
        Vector3 move = (Camera.main.transform.right * x + Camera.main.transform.forward * z);
        move.y = 0; // Vi vil ikke fly oppover

        if (move.magnitude > 0.1f)
        {
            // FLYTTING
            controller.Move(move.normalized * moveSpeed * Time.deltaTime);

            // ØYEBLIKKELIG SVINGING:
            // Vi setter retningen direkte så det ikke blir "trege" svinger
            transform.forward = move;

            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        // HOPP
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        // TYNGDEKRAFT
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}