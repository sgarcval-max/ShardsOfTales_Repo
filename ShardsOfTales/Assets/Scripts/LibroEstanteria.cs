using UnityEngine;
using System.Collections;

public class LibroEstanteria : MonoBehaviour
{
    public string nombreNivel = "Libro";
    public string escenaNivel = "SCN_Flautista_00";
    public Color color = Color.white;
    public Color colorPaginas = new Color(0.96f, 0.92f, 0.8f);

    public Vector3 direccionSalida = new Vector3(0f, 0f, -1f);
    public float distanciaSalida = 0.4f;
    public float velocidad = 10f;
    public float escalaAbierto = 3f;

    [HideInInspector] public bool resaltado;

    private Vector3 posBase;
    private Vector3 escalaBase;
    private Quaternion rotBase;
    private Renderer rend;
    private bool bloqueado;

    void Start()
    {
        posBase = transform.position;
        escalaBase = transform.localScale;
        rotBase = transform.rotation;
        rend = GetComponent<Renderer>();
        rend.material.color = color;
    }

    void Update()
    {
        if (bloqueado) return;

        Vector3 destino = posBase;
        if (resaltado)
            destino += direccionSalida.normalized * distanciaSalida;

        float t = 1f - Mathf.Exp(-velocidad * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, destino, t);
    }

    public IEnumerator Abrir(Vector3 destino)
    {
        bloqueado = true;

        Vector3 desde = transform.position;
        yield return Animar(0.7f, t => transform.position = Vector3.Lerp(desde, destino, t));

        Vector3 escA = escalaBase;
        Vector3 escB = new Vector3(escalaBase.x * escalaAbierto, escalaBase.y, escalaBase.z);
        yield return Animar(0.5f, t =>
        {
            transform.localScale = Vector3.Lerp(escA, escB, t);
            rend.material.color = Color.Lerp(color, colorPaginas, t);
        });
    }

    public IEnumerator Cerrar()
    {
        Vector3 escA = transform.localScale;
        Color colA = rend.material.color;
        yield return Animar(0.4f, t =>
        {
            transform.localScale = Vector3.Lerp(escA, escalaBase, t);
            rend.material.color = Color.Lerp(colA, color, t);
        });

        Vector3 desde = transform.position;
        yield return Animar(0.6f, t => transform.position = Vector3.Lerp(desde, posBase, t));

        transform.rotation = rotBase;
        resaltado = false;
        bloqueado = false;
    }

    IEnumerator Animar(float duracion, System.Action<float> paso)
    {
        float tiempo = 0f;
        while (tiempo < 1f)
        {
            tiempo += Time.deltaTime / duracion;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(tiempo));
            paso(t);
            yield return null;
        }
        paso(1f);
    }
}