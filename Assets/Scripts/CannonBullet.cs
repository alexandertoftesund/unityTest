using UnityEngine;
using System.Collections;

public class CannonBullet : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 10f;

    [Header("Knockback Settings")]
    public float knockbackStrength = 15.0f; // Økt kraften betraktelig
    public float pushDuration = 0.25f;      // Hvor lenge dytten varer

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Vi printer i konsollen så vi VET om den treffer
            Debug.Log("TREFF: Kula traff spilleren!");

            // Vi sjekker både objektet og foreldre for CharacterController
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc == null) cc = other.GetComponentInParent<CharacterController>();

            if (cc != null)
            {
                StopAllCoroutines(); // Stopper tidligere dytter hvis du blir truffet av flere kuler
                StartCoroutine(PushPlayer(cc));
            }
        }
    }

    IEnumerator PushPlayer(CharacterController cc)
    {
        float elapsed = 0;
        // Vi lagrer retningen kula hadde i trefføyeblikket
        Vector3 pushDir = transform.forward;

        while (elapsed < pushDuration)
        {
            // Vi dytter spilleren med høy kraft hver frame i 0.25 sekunder
            // Dette bør overvinne spillerens egne bevegelser
            cc.Move(pushDir * knockbackStrength * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}