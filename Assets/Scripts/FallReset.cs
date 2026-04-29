using UnityEngine;

public class FallReset : MonoBehaviour
{
    private LevelManager levelManager;

    void Start()
    {
        // Vi finner LevelManageren i banen automatisk
        levelManager = GameObject.FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (levelManager != null)
            {
                // Vi bruker funksjonen vi laget i LevelManager
                levelManager.RespawnPlayer();
                Debug.Log("Spilleren falt og ble sendt til siste checkpoint!");
            }
            else
            {
                Debug.LogWarning("Fant ingen LevelManager i scenen!");
            }
        }
    }
}