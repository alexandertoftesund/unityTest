using UnityEngine;

public class FallReset : MonoBehaviour
{
    private LevelManager levelManager;

    void Start()
    {
        FindLevelManager();
    }

    private void FindLevelManager()
    {
        levelManager = FindFirstObjectByType<LevelManager>();

        if (levelManager == null)
        {
            Debug.LogWarning("FallReset fant ikke LevelManager ennå.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Prøv å finne LevelManager på nytt hvis den ikke ble funnet i Start()
            if (levelManager == null)
            {
                FindLevelManager();
            }

            if (levelManager != null)
            {
                levelManager.RespawnPlayer();
                Debug.Log("Spilleren falt og ble sendt til siste checkpoint!");
            }
            else
            {
                Debug.LogWarning("Fant fortsatt ingen LevelManager i scenen!");
            }
        }
    }
}