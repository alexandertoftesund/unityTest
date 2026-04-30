using UnityEngine;

public class PlayerIdleSit : MonoBehaviour
{
    [Header("Idle Sit Settings")]
    public float timeBeforeSitting = 10f;
    public float sitDownTime = 1f;
    public float standUpTime = 1.5f;

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

        // Hvis karakteren setter seg ned, vent til animasjonen er ferdig
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

        // Hvis karakteren reiser seg opp, vent før movement skrus på igjen
        if (isStandingUp)
        {
            actionTimer -= Time.deltaTime;

            if (actionTimer <= 0f)
            {
                isStandingUp = false;

                if (playerMovement != null)
                {
                    playerMovement.canMove = true;
                }
            }

            return;
        }

        // Hvis karakteren sitter, start standup når spilleren trykker movement
        if (isSitting)
        {
            if (pressedKey)
            {
                StartStandingUp();
            }

            return;
        }

        // Hvis spilleren gjør noe, reset idle timer
        if (holdingKey)
        {
            idleTimer = 0f;
            return;
        }

        // Tell hvor lenge spilleren har vært idle
        idleTimer += Time.deltaTime;

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
        isSitting = false;
        isStandingUp = false;

        actionTimer = sitDownTime;

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
        isSittingDown = false;
        isStandingUp = true;

        actionTimer = standUpTime;

        // Movement skal fortsatt være av mens standup-animasjonen spiller
        if (playerMovement != null)
        {
            playerMovement.canMove = false;
        }

        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("Standup");
    }
}