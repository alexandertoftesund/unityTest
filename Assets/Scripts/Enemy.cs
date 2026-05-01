using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    public enum State { Patrol, Wait, Chase, Attack }

    [Header("Bevegelse (Blend Tree verdier)")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 4.0f;
    public float detectionRange = 10f;
    public float attackRange = 1.2f;

    [Header("Auto-Patrulje")]
    [Tooltip("Sett denne over 0 for å lage tilfeldige punkter hvis listen under er tom.")]
    public float autoPatrolRadius = 0f;
    public int autoPointCount = 4;

    [Header("Innstillinger")]
    public float waitAtPointDuration = 2.0f;
    public float attackCooldown = 2.0f;

    [Header("Lyd")]
    public AudioSource audioSource;
    public AudioClip swingSound;
    [Range(0, 1)] public float swingVolume = 0.5f;

    [Header("Referanser")]
    public Transform player;
    public Transform[] patrolPoints = new Transform[0];

    private List<Vector3> activePatrolPoints = new List<Vector3>();
    private NavMeshAgent agent;
    private Animator anim;
    private State currentState = State.Patrol;
    private int currentPointIndex = 0;
    private float waitTimer = 0f;
    private float lastAttackTime;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // Prøver å hente AudioSource automatisk hvis den ikke er lagt inn manuelt
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Lagre startpunktet
        startPosition = transform.position;
        startRotation = transform.rotation;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Juster agenten slik at den stopper i riktig avstand for sverdslag
        agent.stoppingDistance = attackRange - 0.2f;

        SetupPatrolPoints();

        if (activePatrolPoints.Count > 0) GoToNextPoint();
    }

    void SetupPatrolPoints()
    {
        activePatrolPoints.Clear();

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            foreach (Transform t in patrolPoints)
            {
                if (t != null) activePatrolPoints.Add(t.position);
            }
        }

        if (activePatrolPoints.Count == 0 && autoPatrolRadius > 0)
        {
            for (int i = 0; i < autoPointCount; i++)
            {
                activePatrolPoints.Add(GetRandomNavMeshPoint(startPosition, autoPatrolRadius));
            }
            activePatrolPoints.Add(startPosition);
        }
    }

    Vector3 GetRandomNavMeshPoint(Vector3 center, float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            Vector3 randomPos = new Vector3(center.x + randomCircle.x, center.y, center.z + randomCircle.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 1.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return center;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange) currentState = State.Attack;
        else if (distance <= detectionRange) currentState = State.Chase;
        else if (currentState == State.Chase || currentState == State.Attack)
        {
            currentState = State.Patrol;
            GoToNextPoint();
        }

        switch (currentState)
        {
            case State.Patrol:
                agent.isStopped = false;
                agent.speed = walkSpeed;
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
                {
                    currentState = State.Wait;
                    waitTimer = waitAtPointDuration;
                }
                break;

            case State.Wait:
                agent.isStopped = true;
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0)
                {
                    currentState = State.Patrol;
                    GoToNextPoint();
                }
                break;

            case State.Chase:
                agent.isStopped = false;
                agent.speed = runSpeed;
                agent.SetDestination(player.position);
                break;

            case State.Attack:
                AttackLogic();
                break;
        }

        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    void AttackLogic()
    {
        agent.isStopped = true;
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);

        if (Time.time > lastAttackTime + attackCooldown)
        {
            // --- LYD-LOGIKK STARTER HER ---
            if (audioSource != null && swingSound != null)
            {
                // Varierer pitch litt for mer naturlig lyd
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(swingSound, swingVolume);
            }
            // --- LYD-LOGIKK SLUTTER HER ---

            int randomAttack = Random.Range(1, 4);
            anim.SetTrigger("Attack" + randomAttack);
            lastAttackTime = Time.time;
        }
    }

    public void ResetEnemy()
    {
        currentState = State.Patrol;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.Warp(startPosition);
        }

        transform.rotation = startRotation;
        agent.isStopped = false;
        currentPointIndex = 0;

        if (activePatrolPoints.Count > 0) GoToNextPoint();
    }

    void GoToNextPoint()
    {
        if (activePatrolPoints.Count == 0) return;
        agent.SetDestination(activePatrolPoints[currentPointIndex]);
        currentPointIndex = (currentPointIndex + 1) % activePatrolPoints.Count;
    }
}