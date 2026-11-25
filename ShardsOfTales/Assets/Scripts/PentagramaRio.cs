using UnityEngine;

public class PentagramaRio : MonoBehaviour
{
    public Transform[] piedras;
    public Material material;
    public float[] posicionesZ = { 2f, 4f, 6f, 8f, 10f };
    public float anchoLineas = 14f;
    public float alturaY = -0.28f;
    public float grosor = 0.12f;

    void Start()
    {
        foreach (float z in posicionesZ)
        {
            GameObject linea = GameObject.CreatePrimitive(PrimitiveType.Cube);
            linea.name = "Linea_Pentagrama";
            linea.transform.SetParent(transform);
            linea.transform.position = new Vector3(0f, alturaY, z);
            linea.transform.localScale = new Vector3(anchoLineas, 0.05f, grosor);
            Destroy(linea.GetComponent<Collider>());
            linea.GetComponent<Renderer>().sharedMaterial = material;
        }

        foreach (Transform p in piedras)
        {
            GameObject nota = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            nota.name = "Nota_Musical";
            nota.transform.SetParent(p, false);
            nota.transform.localPosition = new Vector3(0f, 1.05f, 0f);
            nota.transform.localScale = new Vector3(0.45f, 0.2f, 0.3f);
            Destroy(nota.GetComponent<Collider>());
            nota.GetComponent<Renderer>().sharedMaterial = material;
        }
    }
}