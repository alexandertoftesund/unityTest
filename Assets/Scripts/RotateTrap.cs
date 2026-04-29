using UnityEngine;

public class RotateTrap : MonoBehaviour
{
    [Header("Rotasjons-innstillinger")]
    // Prøv verdier mellom 30 og 60 for en behagelig fart.
    // Et negativt tall (f.eks. -45) vil få den til å snurre motsatt vei.
    public float rotationSpeed = 45f;

    void Update()
    {
        // Vi roterer rundt Y-aksen (den grønne pilen)
        // Time.deltaTime sørger for at farten er lik uansett bildefrekvens.
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}