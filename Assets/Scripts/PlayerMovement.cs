using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Hvor fort karakteren beveger seg
    public float moveSpeed = 4f;

    // Hvor fort karakteren snur seg mot retningen den går
    public float turnSpeed = 8f;

    // Animator styrer idle- og løpeanimasjonene
    private Animator animator;



    // ----------------- JUMP --------------
    // Hvor høyt karakteren hopper
    public float jumpHeight = 2f;

    // Hvor sterkt karakteren trekkes ned
    public float gravity = -20f;

    // Fart opp/ned
    private float verticalSpeed = 0f;

    // Start-høyden til karakteren
    private float groundY;

    // Om karakteren står på bakken
    private bool isGrounded = true;

    // Om jump-animasjonen har startet, men fysisk hopp ikke har begynt enda
   

    

    private void Awake()
    {
        // Henter Animator-komponenten fra karakteren
        animator = GetComponent<Animator>();

        groundY = transform.position.y;
    }

    private void Update()
    {
        float horizontal = 0f;
        float vertical = 0f;

        // W = fremover i forhold til kameraet
        if (Input.GetKey(KeyCode.W))
        {
            vertical += 1f;
        }

        // S = bakover i forhold til kameraet
        if (Input.GetKey(KeyCode.S))
        {
            vertical -= 1f;
        }

        // D = høyre i forhold til kameraet
        if (Input.GetKey(KeyCode.D))
        {
            horizontal += 1f;
        }

        // A = venstre i forhold til kameraet
        if (Input.GetKey(KeyCode.A))
        {
            horizontal -= 1f;
        }

        // Lager input-retningen fra WASD
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);
        inputDirection = inputDirection.normalized;

        // Sjekker om spilleren trykker på noen movement-knapper
        bool isMoving = inputDirection != Vector3.zero;

        if (isMoving)
        {
            // Henter kameraets fremover-retning
            Vector3 cameraForward = Camera.main.transform.forward;

            // Fjerner opp/ned-retning fra kameraet
            // Vi vil bare bevege oss langs bakken
            cameraForward.y = 0f;
            cameraForward = cameraForward.normalized;

            // Henter kameraets høyre-retning
            Vector3 cameraRight = Camera.main.transform.right;

            // Fjerner opp/ned-retning her også
            cameraRight.y = 0f;
            cameraRight = cameraRight.normalized;

            // Lager faktisk bevegelsesretning basert på kameraet
            Vector3 moveDirection = cameraForward * vertical + cameraRight * horizontal;
            moveDirection = moveDirection.normalized;

            // Flytter karakteren i riktig retning
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            // Finner hvilken vei karakteren skal se
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            // Snur karakteren gradvis mot retningen
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );

            // Fortell Animator at karakteren løper
                animator.SetFloat("Speed", 1f);
            }
            else
            {
                // Fortell Animator at karakteren står stille
                animator.SetFloat("Speed", 0f);
            }



            // -------- JUMP ---------------
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isGrounded = false;

            animator.SetTrigger("Jump");
        }

    // Gravity trekker karakteren ned
    verticalSpeed += gravity * Time.deltaTime;

    // Flytter karakteren opp eller ned
    transform.position += Vector3.up * verticalSpeed * Time.deltaTime;

    // Når karakteren kommer ned til bakken igjen
    if (transform.position.y <= groundY)
    {
        Vector3 position = transform.position;
        position.y = groundY;
        transform.position = position;

        verticalSpeed = 0f;
        isGrounded = true;
    }
    }
}