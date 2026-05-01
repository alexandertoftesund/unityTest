using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyDeath : MonoBehaviour
{
    [Header("Innstillinger")]
    public float shrinkDuration = 0.5f;
    public float bounceForce = 12f;

    private bool isDead = false;
    private GameObject enemyRoot;

    void Start()
    {
        enemyRoot = transform.parent.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Vi sjekker at spilleren lever (ikke er i ferd med å respawne)
        // og at fienden ikke allerede er død.
        if (!isDead && other.CompareTag("Player"))
        {
            isDead = true; // Sett denne MED EN GANG

            // 1. FINN OG DEAKTIVER SVERDET/RESPAWNHANDLER UMIDDELBART
            // Dette hindrer at spilleren dør samtidig som fienden.
            RespawnHandler weapon = enemyRoot.GetComponentInChildren<RespawnHandler>();
            if (weapon != null)
            {
                weapon.enabled = false; // Stopper skriptet
                weapon.gameObject.SetActive(false); // Skjuler sverd-triggeren helt
            }

            // 2. Gi spilleren et hopp
            ApplyBounce(other.gameObject);

            // 3. Start døden
            StartCoroutine(DieSequence());
        }
    }

    void ApplyBounce(GameObject player)
    {
        // Hvis du bruker CharacterController:
        player.SendMessage("Jump", bounceForce, SendMessageOptions.DontRequireReceiver);
    }

    IEnumerator DieSequence()
    {
        // Skru av AI så han ikke fortsetter å gå mens han krymper
        Enemy enemyAI = enemyRoot.GetComponent<Enemy>();
        NavMeshAgent agent = enemyRoot.GetComponent<NavMeshAgent>();
        if (enemyAI != null) enemyAI.enabled = false;
        if (agent != null) agent.isStopped = true;

        Vector3 originalScale = enemyRoot.transform.localScale;
        float elapsed = 0;

        while (elapsed < shrinkDuration)
        {
            enemyRoot.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsed / shrinkDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(enemyRoot);
    }
}