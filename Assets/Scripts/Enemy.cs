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

        // 1. Prioriter manuelle punkter hvis de finnes
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            foreach (Transform t in patrolPoints)
            {
                if (t != null) activePatrolPoints.Add(t.position);
            }
        }

        // 2. Hvis ingen manuelle, generer automatiske punkter "midt på" NavMeshet
        if (activePatrolPoints.Count == 0 && autoPatrolRadius > 0)
        {
            for (int i = 0; i < autoPointCount; i++)
            {
                activePatrolPoints.Add(GetRandomNavMeshPoint(startPosition, autoPatrolRadius));
            }
            activePatrolPoints.Add(startPosition); // Legg alltid til hjemmet som et punkt
        }
    }

    // Forbedret punkt-finner som unngår kantene
    Vector3 GetRandomNavMeshPoint(Vector3 center, float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            // Velger et flatt punkt på en sirkel i stedet for en kule i lufta
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            Vector3 randomPos = new Vector3(center.x + randomCircle.x, center.y, center.z + randomCircle.y);

            NavMeshHit hit;
            // Bruker kort søkeavstand (1.0f) for å unngå at den "snapper" til fjerne kanter
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

        // --- TILSTANDS-LOGIKK ---
        if (distance <= attackRange) currentState = State.Attack;
        else if (distance <= detectionRange) currentState = State.Chase;
        else if (currentState == State.Chase || currentState == State.Attack)
        {
            currentState = State.Patrol;
            GoToNextPoint();
        }

        // --- UTFØRING ---
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

        // Oppdaterer farten i Blend Tree for glidende animasjoner
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
            // Velger en av de tre angreps-triggerne i Animator
            int randomAttack = Random.Range(1, 4);
            anim.SetTrigger("Attack" + randomAttack);
            lastAttackTime = Time.time;
        }
    }

    // Kalles av RespawnHandler når spilleren dør
    public void ResetEnemy()
    {
        currentState = State.Patrol;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.Warp(startPosition); // Teleporterer agenten korrekt tilbake
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