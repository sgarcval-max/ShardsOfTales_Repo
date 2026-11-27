using UnityEngine;
using TMPro;
using System.Collections;

public class RioPartitura : MonoBehaviour
{
    public Transform nina;
    public SpriteRenderer spriteNina;
    public TMP_Text textoMensaje;
    public TMP_Text textoNota;

    public string[] notas = { "Re", "Mi", "La", "Mi", "Fa", "La", "Sol", "Fa", "Mi", "Re" };
    public string escenaSiguiente = "SCN_Tronco_00";
    public bool forzarFlautaParaPruebas = true;
    public bool volverAlInicioAlFallar = true;

    public float separacionPiedras = 1.8f;
    public float alturaPorPaso = 0.3f;
    public float alturaDo = 0.5f;
    public float alturaOrilla = 1.2f;
    public float zNina = -0.4f;
    public int numArboles = 16;
    public int semilla = 7;

    static readonly string[] nombres = { "Do", "Re", "Mi", "Fa", "Sol", "La", "Si" };
    static readonly KeyCode[] teclas =
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4,
        KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7
    };

    private Vector3 posInicio;
    private Vector3 posFinal;
    private Vector3[] posPiedras;
    private int[] pasos;
    private int indice = -1;
    private bool ocupada;
    private bool terminado;
    private Coroutine rutinaNota;

    void Awake()
    {
        if (forzarFlautaParaPruebas) GameState.tieneFlauta = true;
    }

    void Start()
    {
        textoMensaje.gameObject.SetActive(false);
        textoNota.gameObject.SetActive(false);

        int n = notas.Length;
        pasos = new int[n];
        for (int i = 0; i < n; i++)
        {
            pasos[i] = BuscarNota(notas[i]);
            if (pasos[i] < 0)
            {
                Debug.LogError("Nota no válida en la lista: " + notas[i]);
                pasos[i] = 0;
            }
        }

        float x0 = -(n - 1) * 0.5f * separacionPiedras;
        posPiedras = new Vector3[n];
        for (int i = 0; i < n; i++)
        {
            posPiedras[i] = new Vector3(
                x0 + i * separacionPiedras,
                alturaDo + pasos[i] * alturaPorPaso + 0.35f,
                zNina);
        }

        float izqRio = x0 - 1.6f;
        float derRio = x0 + (n - 1) * separacionPiedras + 1.6f;
        posInicio = new Vector3(izqRio - 0.8f, alturaOrilla, zNina);
        posFinal = new Vector3(derRio + 0.8f, alturaOrilla, zNina);

        ConstruirEscenario(izqRio, derRio);

        nina.position = posInicio;
        if (spriteNina != null) spriteNina.flipX = false;
    }

    int BuscarNota(string nombre)
    {
        string limpio = nombre.Trim();
        for (int i = 0; i < nombres.Length; i++)
            if (string.Equals(nombres[i], limpio, System.StringComparison.OrdinalIgnoreCase))
                return i;
        return -1;
    }

    // ---------- Escenario de prueba ----------

    void ConstruirEscenario(float izqRio, float derRio)
    {
        float centroX = (izqRio + derRio) * 0.5f;
        float ancho = derRio - izqRio;

        // Agua
        Crear(PrimitiveType.Cube, "Agua",
            new Vector3(centroX, 1.45f, 1f), new Vector3(ancho, 4.9f, 0.5f),
            new Color(0.48f, 0.74f, 0.82f));

        // Orillas
        Color verdeOrilla = new Color(0.2f, 0.48f, 0.2f);
        Crear(PrimitiveType.Cube, "Orilla_Izq",
            new Vector3(izqRio - 3f, alturaOrilla - 1.1f, 0f), new Vector3(6f, 2.2f, 2.5f), verdeOrilla);
        Crear(PrimitiveType.Cube, "Orilla_Der",
            new Vector3(derRio + 3f, alturaOrilla - 1.1f, 0f), new Vector3(6f, 2.2f, 2.5f), verdeOrilla);

        // Pentagrama: 5 líneas
        for (int k = 0; k < 5; k++)
        {
            int paso = 2 + 2 * k;
            float y = alturaDo + paso * alturaPorPaso;
            Crear(PrimitiveType.Cube, "Linea_Pentagrama",
                new Vector3(centroX, y, 0.6f), new Vector3(ancho - 0.6f, 0.08f, 0.1f),
                new Color(0.75f, 0.95f, 0.9f));
        }

        // Piedras
        for (int i = 0; i < posPiedras.Length; i++)
        {
            Vector3 p = posPiedras[i];
            Crear(PrimitiveType.Sphere, "Piedra_" + notas[i],
                new Vector3(p.x, p.y - 0.35f, 0.2f), new Vector3(1.5f, 0.7f, 0.6f),
                new Color(0.36f, 0.36f, 0.36f));
        }

        // Árboles del fondo
        System.Random rng = new System.Random(semilla);
        Color[] troncos =
        {
            new Color(0.43f, 0.36f, 0.27f),
            new Color(0.48f, 0.4f, 0.31f)
        };
        Color[] copas =
        {
            new Color(0.33f, 0.46f, 0.25f),
            new Color(0.26f, 0.44f, 0.26f),
            new Color(0.34f, 0.5f, 0.32f)
        };

        for (int i = 0; i < numArboles; i++)
        {
            float t = numArboles > 1 ? i / (float)(numArboles - 1) : 0.5f;
            float x = Mathf.Lerp(-24f, 24f, t) + Rango(rng, -1.2f, 1.2f);
            float z = Rango(rng, 4f, 12f);
            float alto = Rango(rng, 6f, 10f);
            float baseY = -1f;

            Crear(PrimitiveType.Cylinder, "Tronco",
                new Vector3(x, baseY + alto * 0.5f, z), new Vector3(0.8f, alto * 0.5f, 0.8f),
                troncos[rng.Next(troncos.Length)]);

            float r = Rango(rng, 2.2f, 3.2f);
            Crear(PrimitiveType.Sphere, "Copa",
                new Vector3(x, baseY + alto + r * 0.3f, z), new Vector3(r * 2f, r * 1.7f, r * 2f),
                copas[rng.Next(copas.Length)]);
        }
    }

    static float Rango(System.Random rng, float a, float b)
    {
        return a + (float)rng.NextDouble() * (b - a);
    }

    GameObject Crear(PrimitiveType tipo, string nombre, Vector3 pos, Vector3 escala, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(tipo);
        go.name = nombre;
        go.transform.SetParent(transform, false);
        go.transform.localPosition = pos;
        go.transform.localScale = escala;
        Destroy(go.GetComponent<Collider>());
        go.GetComponent<Renderer>().material.color = color;
        return go;
    }

    // ---------- Juego ----------

    void Update()
    {
        if (!GameState.tieneFlauta || ocupada || terminado) return;

        for (int i = 0; i < teclas.Length; i++)
        {
            if (Input.GetKeyDown(teclas[i]))
            {
                Tocar(i);
                break;
            }
        }
    }

    void Tocar(int paso)
    {
        if (rutinaNota != null) StopCoroutine(rutinaNota);
        rutinaNota = StartCoroutine(MostrarNota(nombres[paso]));
        StartCoroutine(Intentar(paso));
    }

    IEnumerator Intentar(int paso)
    {
        ocupada = true;

        int siguiente = indice + 1;
        Vector3 origen = nina.position;
        Vector3 destino = posPiedras[siguiente];

        if (paso == pasos[siguiente])
        {
            yield return Saltar(origen, destino);
            indice = siguiente;

            if (indice == posPiedras.Length - 1)
            {
                yield return new WaitForSeconds(0.3f);
                yield return Saltar(nina.position, posFinal);
                yield return Victoria();
                yield break;
            }
        }
        else
        {
            Vector3 fallo = new Vector3(
                destino.x,
                alturaDo + paso * alturaPorPaso + 0.35f,
                zNina);

            yield return Saltar(origen, fallo);
            yield return Caer();
        }

        ocupada = false;
    }

    IEnumerator Saltar(Vector3 a, Vector3 b)
    {
        float dist = Mathf.Abs(b.x - a.x);
        float duracion = 0.35f + dist * 0.08f;
        float altura = 0.8f + dist * 0.12f;

        if (spriteNina != null && dist > 0.01f)
            spriteNina.flipX = b.x < a.x;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            float k = Mathf.Clamp01(t);
            Vector3 p = Vector3.Lerp(a, b, k);
            p.y += 4f * altura * k * (1f - k);
            nina.position = p;
            yield return null;
        }

        nina.position = b;
    }

    IEnumerator Caer()
    {
        float vel = 0f;
        while (nina.position.y > -6f)
        {
            vel += 20f * Time.deltaTime;
            nina.position += Vector3.down * vel * Time.deltaTime;
            yield return null;
        }

        textoMensaje.text = "¡La niña ha caído al agua! Vuelve a empezar.";
        textoMensaje.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.4f);
        textoMensaje.gameObject.SetActive(false);

        if (volverAlInicioAlFallar || indice < 0)
        {
            indice = -1;
            nina.position = posInicio;
            if (spriteNina != null) spriteNina.flipX = false;
        }
        else
        {
            nina.position = posPiedras[indice];
        }
    }

    IEnumerator Victoria()
    {
        terminado = true;
        GameState.ninaCiegaAyudada = true;

        textoMensaje.text = "¡La niña ha cruzado el río sana y salva!";
        textoMensaje.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.5f);

        TransicionPagina.IrA(escenaSiguiente, true);
    }

    IEnumerator MostrarNota(string nombre)
    {
        textoNota.text = "Nota: " + nombre;
        textoNota.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        textoNota.gameObject.SetActive(false);
    }
}
