using UnityEngine;

public class CannonBullet : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 20f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}