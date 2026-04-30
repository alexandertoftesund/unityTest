using UnityEngine;

public class Cannon : MonoBehaviour
{
    public GameObject bullet;
    public Transform cannon;
    public float fireRate = 2.0f;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            InvokeRepeating("Fire", 0f, fireRate);
        }
    }

    void Fire()
    {
        if (bullet != null && cannon != null)
        {
            Instantiate(bullet, cannon.position, cannon.rotation);
        }
    }

    public void StopFiring()
    {
        CancelInvoke("Fire");
    }
}