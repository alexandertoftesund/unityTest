using UnityEngine;

public class PlayerIdleSit : MonoBehaviour
{
    public float timeBeforeSitting = 10f;
    public float sitDownTime = 1f;
    public float standUpTime = 1f;

    private Animator animator;
    private PlayerMovement playerMovement;

    private float idleTimer = 0f;
    private float actionTimer = 0f;

    private bool isSittingDown = false;
    private bool isSitting = false;
    private bool isStandingUp = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        bool holdingKey = IsPressingMovementKey();
        bool pressedKey = PressedMovementKeyThisFrame();

        // Karakteren holder på å sette seg ned
        if (isSittingDown)
        {
            actionTimer -= Time.deltaTime;

            if (actionTimer <= 0f)
            {
                isSittingDown = false;
                isSitting = true;
            }

            return;
        }

        // Karakteren sitter
        if (isSitting)
        {
            if (pressedKey)
            {
                StartStandingUp();
            }

            return;
        }

        // Karakteren holder på å reise seg opp
        if (isStandingUp)
        {
            actionTimer -= Time.deltaTime;

            if (actionTimer <= 0f)
            {
                isStandingUp = false;

                // Movement skrus på først når stand-up er ferdig
                if (playerMovement != null)
                {
                    playerMovement.canMove = true;
                }
            }

            return;
        }

        // Hvis spilleren gjør noe, reset idle-timer
        if (holdingKey)
        {
            idleTimer = 0f;
            return;
        }

        // Tell opp hvor lenge spilleren ikke gjør noe
        idleTimer += Time.deltaTime;

        // Etter nok tid: sett karakteren ned
        if (idleTimer >= timeBeforeSitting)
        {
            StartSittingDown();
        }
    }

    private bool IsPressingMovementKey()
    {
        return Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.Space);
    }

    private bool PressedMovementKeyThisFrame()
    {
        return Input.GetKeyDown(KeyCode.W)
            || Input.GetKeyDown(KeyCode.A)
            || Input.GetKeyDown(KeyCode.S)
            || Input.GetKeyDown(KeyCode.D)
            || Input.GetKeyDown(KeyCode.Space);
    }

    private void StartSittingDown()
    {
        idleTimer = 0f;

        isSittingDown = true;
        actionTimer = sitDownTime;

        // Lås movement mens karakteren setter seg og sitter
        if (playerMovement != null)
        {
            playerMovement.canMove = false;
        }

        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("Sit");
    }

    private void StartStandingUp()
    {
        isSitting = false;
        isStandingUp = true;
        actionTimer = standUpTime;

        animator.SetTrigger("Standup");

        // Ikke skru på movement her.
        // Movement skrus på når standUpTime er ferdig.
    }
}