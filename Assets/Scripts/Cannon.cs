using UnityEngine;

public class Cannon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 2.0f; // Sekunder mellom hver kule

    public void StartFiring()
    {
        // Starter skytingen. 0f betyr "start med en gang",
        // fireRate er hvor ofte den skal gjentas.
        InvokeRepeating("Fire", 0f, fireRate);
    }

    public void StopFiring()
    {
        // Stopper all skyting fra denne kanonen
        CancelInvoke("Fire");
    }

    void Fire()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}