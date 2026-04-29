using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float gravity = -20f;
    public float jumpHeight = 2f;

    // NY: Her styrer du hvor fort karakteren snur seg.
    // Prøv 10-15 for en god følelse.
    public float turnSpeed = 10f;

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
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = (Camera.main.transform.right * x + Camera.main.transform.forward * z);
        move.y = 0;

        if (move.magnitude > 0.1f)
        {
            controller.Move(move.normalized * moveSpeed * Time.deltaTime);

            // --- ENDRING HER: MYK SVINGING ---
            // Vi lager en "mål-rotasjon" basert på retningen vi går
            Quaternion targetRotation = Quaternion.LookRotation(move);

            // Vi bruker Slerp for å bevege oss gradvis fra nåværende rotasjon til målet
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
            // ---------------------------------

            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}