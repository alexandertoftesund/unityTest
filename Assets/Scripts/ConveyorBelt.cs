using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Header("Fysikk")]
    // Navnet er nå 'speed' (ikke beltSpeed) for å matche PlayerMovement
    public float speed = 2.0f;

    // Denne variabelen forteller spilleren hvilken vei han skal dyttes
    [HideInInspector] // Skjuler denne i Inspector så den ikke tar plass, siden den settes i kode
    public Vector3 direction;

    [Header("Visuelt")]
    public float textureSpeed = 0.5f;
    public int materialIndex = 1; // Element 1 i material-listen din

    private Material arrowMaterial;

    void Start()
    {
        // Vi henter ut materialet som skal rulle (pilene)
        Renderer rend = GetComponent<Renderer>();
        if (rend != null && rend.materials.Length > materialIndex)
        {
            arrowMaterial = rend.materials[materialIndex];
        }

        // Vi setter retningen basert på hvordan beltet er rotert i scenen
        UpdateDirection();
    }

    void Update()
    {
        // 1. Visuell rulling av piler
        if (arrowMaterial != null)
        {
            float offset = Time.time * textureSpeed;
            // Vi endrer offset på materialet (Base Map)
            arrowMaterial.SetTextureOffset("_BaseMap", new Vector2(0, -offset));
        }

        // 2. Oppdaterer retningen hver frame i tilfelle beltet roteres
        UpdateDirection();
    }

    void UpdateDirection()
    {
        // Vi bruker -transform.forward slik du hadde det i ditt forrige script
        direction = -transform.forward;
    }

    // MERK: OnTriggerStay er fjernet.
    // Spilleren henter nå verdiene 'speed' og 'direction' herfra automatisk
    // via Raycast i PlayerMovement-scriptet.
}