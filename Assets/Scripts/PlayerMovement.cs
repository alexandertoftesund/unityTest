using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float gravity = -20f;
    public float jumpHeight = 2f;
    public float turnSpeed = 10f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Animator animator;

    // Kraften vi får fra transportbåndet
    private Vector3 conveyorForce = Vector3.zero;

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

        // 1. Spillerens egen bevegelse (WASD)
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = (Camera.main.transform.right * x + Camera.main.transform.forward * z);
        inputDir.y = 0;

        Vector3 playerMove = Vector3.zero;
        if (inputDir.magnitude > 0.1f)
        {
            playerMove = inputDir.normalized * moveSpeed;
            Quaternion targetRotation = Quaternion.LookRotation(inputDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        // 2. Hopp-logikk
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        // 3. Tyngdekraft
        velocity.y += gravity * Time.deltaTime;

        // --- DEN VIKTIGE SAMMENSLAINGEN ---
        // Vi legger sammen ALT: Gange + Hopp + Kraft fra beltet
        Vector3 finalMovement = playerMove + conveyorForce + velocity;

        controller.Move(finalMovement * Time.deltaTime);
    }

    // Når spilleren er inne i trigger-boksen til et belte
    private void OnTriggerStay(Collider other)
    {
        ConveyorBelt belt = other.GetComponent<ConveyorBelt>();
        if (belt != null)
        {
            conveyorForce = belt.direction * belt.speed;
        }
    }

    // Når spilleren hopper ut av eller forlater boksen
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ConveyorBelt>() != null)
        {
            conveyorForce = Vector3.zero;
        }
    }
}