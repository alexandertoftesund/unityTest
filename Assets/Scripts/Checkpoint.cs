using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Color activeColor = Color.green;
    public GameObject activationLight; // Valgfritt: Et lys-objekt som skrus på

    private LevelManager levelManager;
    private bool isActivated = false;
    private Renderer flagRenderer;

    void Start()
    {
        levelManager = GameObject.FindObjectOfType<LevelManager>();
        flagRenderer = GetComponentInChildren<Renderer>(); // Finn mesh-en til flagget

        if (activationLight != null) activationLight.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            ActivateCheckpoint();
        }
    }

    void ActivateCheckpoint()
    {
        isActivated = true;

        // 1. Lagre posisjonen (vi legger til litt høyde så man ikke spawner inni gulvet)
        LevelManager.Instance.UpdateSpawnPoint(transform.position, transform.rotation);

        // 2. Endre farge på flagget
        if (flagRenderer != null)
        {
            flagRenderer.material.color = activeColor;
        }

        // 3. Skru på lys/partikler hvis du har det
        if (activationLight != null)
        {
            activationLight.SetActive(true);
        }
    }
}