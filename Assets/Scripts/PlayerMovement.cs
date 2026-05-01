using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float gravity = -20f;
    public float jumpHeight = 2f;
    public float turnSpeed = 10f;

    [Header("Lyd")]
    public AudioSource audioSource;
    public AudioClip footstepSound;
    public AudioClip landSound;
    [Range(0, 1)] public float footstepVolume = 0.3f;
    [Range(0, 1)] public float landVolume = 0.5f; // NY: Egen kontroll for landingsvolum
    public float footstepInterval = 0.35f;

    public bool canMove = true;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool wasGrounded;
    private Animator animator;
    private Vector3 conveyorForce = Vector3.zero;

    private float nextFootstepTime;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        wasGrounded = true;
    }

    void Update()
    {
        if (!canMove)
        {
            animator.SetFloat("Speed", 0f);
            velocity = Vector3.zero;
            conveyorForce = Vector3.zero;
            return;
        }

        isGrounded = controller.isGrounded;

        // Sjekker om vi har landet denne framen
        if (isGrounded && !wasGrounded)
        {
            if (velocity.y < -1f && audioSource != null && landSound != null)
            {
                // Bruker landVolume-variabelen her
                audioSource.PlayOneShot(landSound, landVolume);
            }
        }
        wasGrounded = isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

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

            if (isGrounded && Time.time > nextFootstepTime)
            {
                if (audioSource != null && footstepSound != null)
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(footstepSound, footstepVolume);
                }
                nextFootstepTime = Time.time + footstepInterval;
            }
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
        Vector3 finalMovement = playerMove + conveyorForce + velocity;
        controller.Move(finalMovement * Time.deltaTime);
    }

    public void Jump(float force)
    {
        velocity.y = force;
        if (animator != null) animator.SetTrigger("Jump");
    }

    private void OnTriggerStay(Collider other)
    {
        ConveyorBelt belt = other.GetComponent<ConveyorBelt>();
        if (belt != null) conveyorForce = belt.direction * belt.speed;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ConveyorBelt>() != null) conveyorForce = Vector3.zero;
    }
}