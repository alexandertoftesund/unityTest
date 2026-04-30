using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Color activeColor = Color.green;
    public GameObject activationLight; // Valgfritt: Et lys-objekt som skrus på

    private bool isActivated = false;
    private Renderer flagRenderer;

    void Start()
    {
        // Vi trenger ikke lenger FindObjectOfType her
        flagRenderer = GetComponentInChildren<Renderer>(); // Finn mesh-en til flagget

        if (activationLight != null)
        {
            activationLight.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Sjekker om det er spilleren og om checkpointen allerede er brukt
        if (other.CompareTag("Player") && !isActivated)
        {
            ActivateCheckpoint();
        }
    }

    void ActivateCheckpoint()
    {
        isActivated = true;

        // 1. Lagre posisjonen og rotasjonen direkte i LevelManager.Instance
        LevelManager.Instance.UpdateSpawnPoint(transform.position, transform.rotation);

        // 2. Endre farge på flagget
        if (flagRenderer != null)
        {
            flagRenderer.material.color = activeColor;
        }

        // 3. Skru på lys/partikler
        if (activationLight != null)
        {
            activationLight.SetActive(true);
        }
    }
}