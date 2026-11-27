using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public enum Ingrediente { Ninguno, Espaguetis, Tomate, Albondigas }

public class MinijuegoCocina : MonoBehaviour
{
    public PlayerCocina player;
    public TMP_Text textoInfo;
    public TMP_Text textoAviso;

    public float radioInteraccion = 2.6f;
    public float tiempoCocinar = 3f;

    class Punto
    {
        public string nombre;
        public Ingrediente ing;
        public bool esFuente;
        public Vector3 pos;
        public GameObject marcador;
        public bool hecho;
    }

    static readonly string[] conArticulo = { "", "los espaguetis", "el tomate", "las albóndigas" };
    static readonly string[] titulo = { "nada", "Espaguetis", "Tomate", "Albóndigas" };

    static readonly Color[] colorIng =
    {
        Color.white,
        new Color(0.95f, 0.85f, 0.35f),
        new Color(0.85f, 0.15f, 0.15f),
        new Color(0.5f, 0.3f, 0.15f)
    };

    private List<Punto> puntos = new List<Punto>();
    private GameObject llevadoMarcador;
    private Ingrediente llevando = Ingrediente.Ninguno;
    private bool empezado;
    private bool cocinando;
    private string mensajeTemporal;
    private float mensajeHasta;

    void Start()
    {
        ConstruirCocina();

        llevadoMarcador = Crear(PrimitiveType.Cube, "Ingrediente_Llevado",
            Vector3.zero, new Vector3(0.6f, 0.6f, 0.6f), Color.white, false);
        llevadoMarcador.SetActive(false);

        textoAviso.gameObject.SetActive(false);

        if (GameState.cocinaCompletada)
        {
            foreach (Punto p in puntos)
            {
                p.hecho = true;
                p.marcador.SetActive(!p.esFuente);
            }
            empezado = true;
        }

        RefrescarHud();
    }

    // ---------- Escenario de prueba ----------

    void ConstruirCocina()
    {
        Color crema = new Color(1f, 0.95f, 0.78f);
        Color naranja = new Color(1f, 0.7f, 0.35f);

        for (int ix = 0; ix < 12; ix++)
        {
            for (int iz = 0; iz < 7; iz++)
            {
                Crear(PrimitiveType.Cube, "Baldosa",
                    new Vector3(-11f + 2f * ix, -0.1f, -6f + 2f * iz),
                    new Vector3(2f, 0.2f, 2f),
                    (ix + iz) % 2 == 0 ? crema : naranja, false);
            }
        }

        Color pared = new Color(0.45f, 0.3f, 0.2f);
        Crear(PrimitiveType.Cube, "Pared_Norte", new Vector3(0f, 0.75f, 7.5f), new Vector3(26f, 1.5f, 1f), pared, true);
        Crear(PrimitiveType.Cube, "Pared_Sur", new Vector3(0f, 0.75f, -7.5f), new Vector3(26f, 1.5f, 1f), pared, true);
        Crear(PrimitiveType.Cube, "Pared_Oeste", new Vector3(-12.5f, 0.75f, 0f), new Vector3(1f, 1.5f, 16f), pared, true);
        Crear(PrimitiveType.Cube, "Pared_Este", new Vector3(12.5f, 0.75f, 0f), new Vector3(1f, 1.5f, 16f), pared, true);

        // Dónde se dejan los ingredientes (arriba)
        AnadirPunto("la olla", Ingrediente.Espaguetis, false, new Vector3(-6f, 0f, 5.5f), new Color(0.5f, 0.5f, 0.55f));
        AnadirPunto("la sartén", Ingrediente.Albondigas, false, new Vector3(0f, 0f, 5.5f), new Color(0.2f, 0.2f, 0.22f));
        AnadirPunto("el cazo", Ingrediente.Tomate, false, new Vector3(6f, 0f, 5.5f), new Color(0.85f, 0.4f, 0.2f));

        // Dónde se cogen los ingredientes (abajo)
        AnadirPunto("la despensa", Ingrediente.Espaguetis, true, new Vector3(-9f, 0f, -5f), new Color(0.4f, 0.28f, 0.2f));
        AnadirPunto("la caja de carne", Ingrediente.Albondigas, true, new Vector3(0f, 0f, -5f), new Color(0.6f, 0.65f, 0.7f));
        AnadirPunto("la nevera", Ingrediente.Tomate, true, new Vector3(9f, 0f, -5f), new Color(0.8f, 0.9f, 0.95f));
    }

    void AnadirPunto(string nombre, Ingrediente ing, bool esFuente, Vector3 centro, Color colorMueble)
    {
        Crear(PrimitiveType.Cube, nombre, new Vector3(centro.x, 0.5f, centro.z),
            new Vector3(3f, 1f, 2f), colorMueble, true);

        Punto p = new Punto();
        p.nombre = nombre;
        p.ing = ing;
        p.esFuente = esFuente;
        p.pos = new Vector3(centro.x, 0f, centro.z);
        p.marcador = Crear(PrimitiveType.Cube, "Marca_" + titulo[(int)ing],
            new Vector3(centro.x, 1.2f, centro.z), new Vector3(1.1f, 0.3f, 1.1f),
            colorIng[(int)ing], false);
        p.marcador.SetActive(esFuente);
        puntos.Add(p);
    }

    GameObject Crear(PrimitiveType tipo, string nombre, Vector3 pos, Vector3 escala, Color color, bool conCollider)
    {
        GameObject go = GameObject.CreatePrimitive(tipo);
        go.name = nombre;
        go.transform.SetParent(transform, false);
        go.transform.localPosition = pos;
        go.transform.localScale = escala;
        if (!conCollider) Destroy(go.GetComponent<Collider>());
        go.GetComponent<Renderer>().material.color = color;
        return go;
    }

    // ---------- Juego ----------

    public void Empezar()
    {
        if (GameState.cocinaCompletada)
        {
            MensajeTemporal("La cena ya está lista. ¡Gracias!");
            return;
        }

        empezado = true;
        RefrescarHud();
    }

    void Update()
    {
        if (llevadoMarcador.activeSelf)
            llevadoMarcador.transform.position = player.transform.position + new Vector3(0f, 1.8f, 0f);

        if (cocinando || !player.enabled) return;

        Punto cercano = BuscarCercano();

        string aviso = null;
        if (cercano != null && !GameState.cocinaCompletada)
            aviso = TextoAviso(cercano);
        if (Time.time < mensajeHasta)
            aviso = mensajeTemporal;

        MostrarAviso(aviso);

        if (cercano != null && Input.GetKeyDown(KeyCode.E) && !GameState.cocinaCompletada)
            Interactuar(cercano);
    }

    Punto BuscarCercano()
    {
        Punto mejor = null;
        float mejorDist = radioInteraccion;
        Vector3 pp = player.transform.position;

        foreach (Punto p in puntos)
        {
            if (p.hecho) continue;

            float d = Vector2.Distance(new Vector2(pp.x, pp.z), new Vector2(p.pos.x, p.pos.z));
            if (d <= mejorDist)
            {
                mejorDist = d;
                mejor = p;
            }
        }

        return mejor;
    }

    string TextoAviso(Punto p)
    {
        if (!empezado) return "Habla primero con la señora Potts";

        if (p.esFuente)
        {
            if (llevando != Ingrediente.Ninguno)
                return "Ya llevas " + conArticulo[(int)llevando] + ". Déjalo antes en su sitio.";
            return "Pulsa E para coger " + conArticulo[(int)p.ing];
        }

        if (llevando == Ingrediente.Ninguno)
            return "Es " + p.nombre + ": aquí hay que dejar " + conArticulo[(int)p.ing];
        return "Pulsa E para dejar " + conArticulo[(int)llevando] + " en " + p.nombre;
    }

    void Interactuar(Punto p)
    {
        if (!empezado)
        {
            MensajeTemporal("Habla primero con la señora Potts.");
            return;
        }

        if (p.esFuente)
        {
            if (llevando != Ingrediente.Ninguno)
            {
                MensajeTemporal("Ya llevas " + conArticulo[(int)llevando] + ". Déjalo antes en su sitio.");
                return;
            }

            llevando = p.ing;
            p.hecho = true;
            p.marcador.SetActive(false);

            llevadoMarcador.GetComponent<Renderer>().material.color = colorIng[(int)llevando];
            llevadoMarcador.SetActive(true);
            RefrescarHud();
            return;
        }

        if (llevando == Ingrediente.Ninguno) return;

        if (llevando == p.ing)
        {
            p.hecho = true;
            p.marcador.SetActive(true);
            llevando = Ingrediente.Ninguno;
            llevadoMarcador.SetActive(false);
            RefrescarHud();

            if (TodoColocado())
                StartCoroutine(Cocinar());
        }
        else
        {
            MensajeTemporal("Eso no va ahí. ¡Prueba en otro sitio!");
        }
    }

    bool TodoColocado()
    {
        foreach (Punto p in puntos)
            if (!p.esFuente && !p.hecho) return false;
        return true;
    }

    bool Colocado(Ingrediente ing)
    {
        foreach (Punto p in puntos)
            if (!p.esFuente && p.ing == ing) return p.hecho;
        return false;
    }

    IEnumerator Cocinar()
    {
        cocinando = true;
        player.enabled = false;

        textoAviso.text = "Cocinando...";
        textoAviso.gameObject.SetActive(true);

        float t = 0f;
        while (t < tiempoCocinar)
        {
            t += Time.deltaTime;
            float escala = 1f + 0.25f * Mathf.Abs(Mathf.Sin(t * 8f));

            foreach (Punto p in puntos)
                if (!p.esFuente)
                    p.marcador.transform.localScale = new Vector3(1.1f * escala, 0.3f, 1.1f * escala);

            yield return null;
        }

        foreach (Punto p in puntos)
            if (!p.esFuente)
                p.marcador.transform.localScale = new Vector3(1.1f, 0.3f, 1.1f);

        GameState.cocinaCompletada = true;
        RefrescarHud();

        textoAviso.text = "¡La cena de Bella y la Bestia está lista!\nLa señora Potts está encantada.";
        yield return new WaitForSeconds(3f);
        textoAviso.gameObject.SetActive(false);

        player.enabled = true;
        cocinando = false;
    }

    // ---------- Textos ----------

    void RefrescarHud()
    {
        if (GameState.cocinaCompletada)
        {
            textoInfo.text = "¡La cena está lista! Ya puedes volver al castillo";
            return;
        }

        if (!empezado)
        {
            textoInfo.text = "Habla con la señora Potts";
            return;
        }

        string t = "";
        for (int i = 1; i <= 3; i++)
            t += titulo[i] + " " + (Colocado((Ingrediente)i) ? "[x]" : "[ ]") + "     ";
        t += "·     Llevas: " + titulo[(int)llevando];
        textoInfo.text = t;
    }

    void MostrarAviso(string texto)
    {
        if (texto == null)
        {
            textoAviso.gameObject.SetActive(false);
            return;
        }

        textoAviso.text = texto;
        textoAviso.gameObject.SetActive(true);
    }

    void MensajeTemporal(string texto)
    {
        mensajeTemporal = texto;
        mensajeHasta = Time.time + 2.2f;
    }
}