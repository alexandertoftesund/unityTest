using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Header("Fysikk")]
    public float beltSpeed = 1f;

    [Header("Visuelt")]
    public float textureSpeed = 0.5f;
    public int materialIndex = 1; // Her velger vi materialet med pilene (Element 1)

    private Material arrowMaterial;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();

        // Vi henter ut listen over alle materialer på objektet
        if (rend.materials.Length > materialIndex)
        {
            // Vi lagrer en referanse til akkurat det materialet som skal rulle
            arrowMaterial = rend.materials[materialIndex];
        }
    }

    void Update()
    {
        if (arrowMaterial != null)
        {
            float offset = Time.time * textureSpeed;

            // Vi endrer bare på pil-materialet
            arrowMaterial.SetTextureOffset("_BaseMap", new Vector2(0, -offset));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.Move(-transform.forward * beltSpeed * Time.deltaTime);
            }
        }
    }
}