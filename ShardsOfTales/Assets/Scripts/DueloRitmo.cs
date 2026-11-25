using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DueloRitmo : MonoBehaviour
{
    public RectTransform panelNotas;
    public RectTransform[] receptores;
    public RectTransform barraVida;
    public TMP_Text textoMensaje;
    public TMP_Text textoPuntos;

    public float bpm = 110f;
    public float tiempoViaje = 1.8f;
    public float ventana = 0.18f;
    public float yInicio = 650f;
    public float vidaInicial = 0.5f;
    public float vidaAcierto = 0.03f;
    public float vidaFallo = 0.07f;
    public string escenaFinal = "SCN_Hub_00";

    // 0 a 3 = carril de la nota. El punto es un silencio.
    public string patron = "0.1.2.3.1.2.0.3.0123.3210.0.2.1.3.0.1.2.3.1.0.3.2.0.1.1.2.2.3.3.0.0.1230.3210.0.2.1.3.0.1.2.3.1.3.0.";

    public Color[] colorCarril =
    {
        new Color(0.7f, 0.25f, 0.9f),
        new Color(0f, 0.85f, 1f),
        new Color(0.25f, 0.9f, 0.25f),
        new Color(0.95f, 0.2f, 0.25f)
    };

    class Nota
    {
        public int carril;
        public float tiempo;
        public RectTransform rt;
        public bool resuelta;
    }

    private static bool esReintento;

    private List<Nota> notas = new List<Nota>();
    private int siguienteSpawn;
    private float tiempoCancion;
    private float ultimoTiempo;
    private float vida;
    private int aciertos;
    private int fallos;
    private bool jugando;

    void Start()
    {
        vida = vidaInicial;

        float paso = 60f / bpm / 2f;
        for (int i = 0; i < patron.Length; i++)
        {
            char c = patron[i];
            if (c >= '0' && c <= '3')
            {
                Nota n = new Nota();
                n.carril = c - '0';
                n.tiempo = i * paso;
                notas.Add(n);
                ultimoTiempo = n.tiempo;
            }
        }

        ActualizarUI();
        StartCoroutine(Secuencia());
    }

    IEnumerator Secuencia()
    {
        if (esReintento)
        {
            Mostrar("¡Otra vez!");
            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            Mostrar("El flautista te reta a un duelo musical.\nPulsa A S W D (o las flechas) al ritmo.");
            yield return new WaitForSeconds(3.5f);
        }

        for (int i = 3; i >= 1; i--)
        {
            Mostrar(i.ToString());
            yield return new WaitForSeconds(1f);
        }

        Mostrar("¡Ya!");
        tiempoCancion = -tiempoViaje;
        jugando = true;

        yield return new WaitForSeconds(0.8f);
        if (jugando) textoMensaje.gameObject.SetActive(false);
    }

    void Update()
    {
        for (int c = 0; c < 4; c++)
        {
            Color col = colorCarril[c];
            col.a = Mantenida(c) ? 1f : 0.35f;
            receptores[c].GetComponent<Image>().color = col;
        }

        if (!jugando) return;

        tiempoCancion += Time.deltaTime;

        while (siguienteSpawn < notas.Count &&
               notas[siguienteSpawn].tiempo - tiempoCancion <= tiempoViaje)
        {
            CrearVisual(notas[siguienteSpawn]);
            siguienteSpawn++;
        }

        for (int c = 0; c < 4; c++)
            if (Pulsada(c)) Intentar(c);

        float yReceptor = receptores[0].anchoredPosition.y;

        for (int i = 0; i < siguienteSpawn; i++)
        {
            Nota n = notas[i];
            if (n.resuelta) continue;

            float restante = n.tiempo - tiempoCancion;
            float y = yReceptor + (restante / tiempoViaje) * (yInicio - yReceptor);
            float x = receptores[n.carril].anchoredPosition.x;
            n.rt.anchoredPosition = new Vector2(x, y);

            if (restante < -ventana) Fallo(n);
        }

        if (vida <= 0f)
        {
            StartCoroutine(Final(false));
            return;
        }

        if (siguienteSpawn >= notas.Count && tiempoCancion > ultimoTiempo + ventana + 0.5f)
            StartCoroutine(Final(true));
    }

    bool Pulsada(int c)
    {
        switch (c)
        {
            case 0: return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
            case 1: return Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
            case 2: return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
            default: return Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        }
    }

    bool Mantenida(int c)
    {
        switch (c)
        {
            case 0: return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
            case 1: return Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
            case 2: return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            default: return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        }
    }

    void Intentar(int carril)
    {
        Nota mejor = null;
        float mejorDif = ventana;

        for (int i = 0; i < siguienteSpawn; i++)
        {
            Nota n = notas[i];
            if (n.resuelta || n.carril != carril) continue;

            float dif = Mathf.Abs(n.tiempo - tiempoCancion);
            if (dif <= mejorDif)
            {
                mejorDif = dif;
                mejor = n;
            }
        }

        if (mejor != null) Acierto(mejor);
    }

    void Acierto(Nota n)
    {
        n.resuelta = true;
        Destroy(n.rt.gameObject);
        aciertos++;
        vida = Mathf.Clamp01(vida + vidaAcierto);
        ActualizarUI();
    }

    void Fallo(Nota n)
    {
        n.resuelta = true;
        Destroy(n.rt.gameObject);
        fallos++;
        vida = Mathf.Clamp01(vida - vidaFallo);
        ActualizarUI();
    }

    void CrearVisual(Nota n)
    {
        GameObject go = new GameObject("Nota", typeof(RectTransform), typeof(Image));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(panelNotas, false);
        rt.sizeDelta = new Vector2(110f, 110f);
        rt.SetAsFirstSibling();

        Image img = go.GetComponent<Image>();
        img.color = colorCarril[n.carril];
        img.raycastTarget = false;

        n.rt = rt;
    }

    void ActualizarUI()
    {
        barraVida.localScale = new Vector3(vida, 1f, 1f);
        textoPuntos.text = "Aciertos: " + aciertos + "   Fallos: " + fallos;
    }

    void Mostrar(string texto)
    {
        textoMensaje.text = texto;
        textoMensaje.gameObject.SetActive(true);
    }

    IEnumerator Final(bool bueno)
    {
        jugando = false;

        foreach (Nota n in notas)
            if (n.rt != null) Destroy(n.rt.gameObject);

        if (bueno)
        {
            esReintento = false;
            GameState.flautistaCompletado = true;
            GameState.finalFlautistaBueno = true;

            Mostrar("¡Has ganado el duelo!\nLos niños vuelven a sus casas, felices.");
            yield return new WaitForSeconds(5f);
            SceneManager.LoadScene(escenaFinal);
        }
        else
        {
            esReintento = true;

            Mostrar("El flautista se ríe de ti...\n¡Inténtalo otra vez!");
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
