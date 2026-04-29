using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    [Header("Visuelt")]
    public float rotationSpeed = 50f;
    public GameObject collectEffect; // Valgfritt: Partikler eller lys som dukker opp

    private bool hasReachedGoal = false;

    void Update()
    {
        // Får diamanten til å snurre rolig rundt
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasReachedGoal)
        {
            hasReachedGoal = true;
            FinishLevel();
        }
    }

    void FinishLevel()
    {
        Debug.Log("Mål nådd!");

        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // Vi bruker vår globale LevelManager til å bytte bane
        LevelManager.Instance.LoadNextLevel();

        // Skjul diamanten mens vi venter på at neste bane laster
        gameObject.SetActive(false);
    }
}