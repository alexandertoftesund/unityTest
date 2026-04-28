using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    [Header("Innstillinger")]
    [Tooltip("Hvor mye liv spilleren mister ved berøring")]
    public int damageAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            Debug.Log("AU! Spilleren traff piggene og mistet " + damageAmount + " liv.");
        }
    }
}