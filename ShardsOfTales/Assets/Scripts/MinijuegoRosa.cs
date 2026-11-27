using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MinijuegoRosa : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text textoInfo;
    public TMP_Text textoMensaje;
    public MonoBehaviour player;

    public int totalMoscas = 10;
    public int toquesMaximos = 3;
    public int maxActivas = 4;
    public float velocidadInicial = 170f;
    public float incrementoVelocidad = 15f;
    public float intervaloInicial = 2.2f;
    public float intervaloMinimo = 0.9f;
    public float reduccionIntervalo = 0.12f;
    public string teclasPosibles = "ASDFQWERZXCV";
    public bool reintentarAlPerder = true;
    public string escenaFinal = "SCN_Hub_00";

    public Vector2 centroRosa = new Vector2(0f, 40f);
    public float radioRosa = 80f;

    class Mosca
    {
        public RectTransform rt;
        public RectTransform alaIzq;
        public RectTransform alaDer;
        public char tecla;
        public float fase;
        public bool muriendo;
    }

    static readonly Color RojoFlor = new Color(0.9f, 0.15f, 0.3f);
    static readonly Color RojoCentro = new Color(0.65f, 0.08f, 0.22f);
    static readonly Color FlorMarchita = new Color(0.32f, 0.2f, 0.22f);
    static readonly Color CentroMarchito = new Color(0.22f, 0.13f, 0.15f);

    private RectTransform contenedor;
    private Sprite circulo;
    private Image flor;
    private Image florCentro;
    private List<Mosca> moscas = new List<Mosca>();

    private bool activo;
    private bool jugando;
    private bool victoria;
    private int derrotadas;
    private int toques;
    private float intervalo;
    private float tiempoSpawn;

    Vector2 PuntoRosa
    {
        get { return centroRosa + new Vector2(0f, 70f); }
    }

    void Start()
    {
        contenedor = panel.GetComponent<RectTransform>();
        circulo = CrearCirculo(128);

        DibujarRosa();

        textoInfo.transform.SetAsLastSibling();
        textoMensaje.transform.SetAsLastSibling();
        textoMensaje.gameObject.SetActive(false);

        panel.SetActive(false);
    }

    // ---------- Dibujo ----------

    static Sprite CrearCirculo(int tam)
    {
        Texture2D tex = new Texture2D(tam, tam, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;

        float c = (tam - 1) * 0.5f;
        Color[] px = new Color[tam * tam];

        for (int y = 0; y < tam; y++)
        {
            for (int x = 0; x < tam; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(c, c)) / c;
                float alpha = Mathf.Clamp01((1f - d) * c);
                px[y * tam + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(px);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, tam, tam), new Vector2(0.5f, 0.5f), 100f);
    }

    Image Img(RectTransform padre, string nombre, Vector2 pos, Vector2 tam, Color color, Sprite sprite)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform), typeof(Image));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(padre, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = tam;

        Image img = go.GetComponent<Image>();
        img.sprite = sprite;
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    void DibujarRosa()
    {
        Vector2 c = centroRosa;
        Color verde = new Color(0.2f, 0.6f, 0.3f);

        Img(contenedor, "Pedestal", c + new Vector2(0f, -310f), new Vector2(480f, 150f),
            new Color(0.35f, 0.28f, 0.32f), null);
        Img(contenedor, "Tallo", c + new Vector2(0f, -70f), new Vector2(14f, 260f), verde, null);

        Image h1 = Img(contenedor, "Hoja_1", c + new Vector2(-45f, -30f), new Vector2(76f, 34f), verde, circulo);
        h1.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 25f);
        Image h2 = Img(contenedor, "Hoja_2", c + new Vector2(45f, -60f), new Vector2(76f, 34f), verde, circulo);
        h2.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -25f);

        flor = Img(contenedor, "Flor", c + new Vector2(0f, 70f), new Vector2(130f, 130f), RojoFlor, circulo);
        florCentro = Img(contenedor, "Flor_Centro", c + new Vector2(0f, 70f), new Vector2(70f, 70f), RojoCentro, circulo);

        Img(contenedor, "Jarron", c + new Vector2(0f, -155f), new Vector2(170f, 170f),
            new Color(0.6f, 0.8f, 0.95f, 0.35f), null);
    }

    Mosca CrearMosca(char tecla, Vector2 pos)
    {
        Image cuerpo = Img(contenedor, "Mosca", pos, new Vector2(56f, 56f), new Color(0.12f, 0.12f, 0.15f), circulo);
        RectTransform rt = cuerpo.rectTransform;

        Color colorAla = new Color(0.8f, 0.92f, 1f, 0.65f);
        Image ai = Img(rt, "AlaIzq", new Vector2(-28f, 22f), new Vector2(46f, 30f), colorAla, circulo);
        Image ad = Img(rt, "AlaDer", new Vector2(28f, 22f), new Vector2(46f, 30f), colorAla, circulo);
        Img(rt, "OjoIzq", new Vector2(-10f, 8f), new Vector2(14f, 14f), new Color(0.9f, 0.15f, 0.15f), circulo);
        Img(rt, "OjoDer", new Vector2(10f, 8f), new Vector2(14f, 14f), new Color(0.9f, 0.15f, 0.15f), circulo);

        GameObject lg = new GameObject("Letra", typeof(RectTransform));
        lg.transform.SetParent(rt, false);
        TextMeshProUGUI tmp = lg.AddComponent<TextMeshProUGUI>();
        tmp.font = textoInfo.font;
        tmp.text = tecla.ToString();
        tmp.fontSize = 64f;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(1f, 0.95f, 0.4f);
        tmp.raycastTarget = false;
        tmp.rectTransform.anchoredPosition = new Vector2(0f, 62f);
        tmp.rectTransform.sizeDelta = new Vector2(100f, 80f);

        Mosca m = new Mosca();
        m.rt = rt;
        m.alaIzq = ai.rectTransform;
        m.alaDer = ad.rectTransform;
        m.tecla = tecla;
        m.fase = Random.Range(0f, 6.28f);
        return m;
    }

    // ---------- Juego ----------

    public void Iniciar()
    {
        if (activo) return;

        if (GameState.rosaSalvada)
            StartCoroutine(YaSalvada());
        else
            StartCoroutine(Partida());
    }

    IEnumerator YaSalvada()
    {
        activo = true;
        player.enabled = false;
        panel.SetActive(true);

        Reiniciar();
        textoInfo.text = "La rosa está a salvo";
        Mostrar("La rosa luce preciosa. ¡Gracias por cuidarla!");
        yield return new WaitForSeconds(2.5f);
        Ocultar();

        panel.SetActive(false);
        player.enabled = true;
        activo = false;
    }

    IEnumerator Partida()
    {
        activo = true;
        player.enabled = false;
        panel.SetActive(true);

        bool primera = true;

        while (true)
        {
            Reiniciar();

            Mostrar(primera
                ? "Pulsa la tecla que lleva cada mosca para derrotarla\nantes de que toque la rosa."
                : "¡Otra vez! Protege la rosa.");
            yield return new WaitForSeconds(primera ? 3.5f : 1.8f);
            Ocultar();

            victoria = false;
            jugando = true;
            while (jugando) yield return null;

            LimpiarMoscas();

            if (victoria) break;

            if (reintentarAlPerder)
            {
                Mostrar("La rosa se ha marchitado...\n¡Inténtalo de nuevo!");
                yield return new WaitForSeconds(3f);
                primera = false;
            }
            else
            {
                GameState.bellaBestiaCompletado = true;
                GameState.finalBellaBestiaBueno = false;
                Mostrar("La rosa se ha marchitado y el maleficio no se rompe...\n(Final malo)");
                yield return new WaitForSeconds(5f);
                SceneManager.LoadScene(escenaFinal);
                yield break;
            }
        }

        GameState.rosaSalvada = true;
        GameState.bellaBestiaCompletado = true;
        GameState.finalBellaBestiaBueno = true;
        Mostrar("¡Has salvado la rosa!\nBella y la Bestia hacen las paces y el maleficio se rompe.");
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(escenaFinal);
    }

    void Update()
    {
        if (!jugando) return;

        float dt = Time.deltaTime;

        // Teclas
        foreach (char ch in Input.inputString)
        {
            if (char.IsLetter(ch)) Intentar(char.ToUpper(ch));
        }

        // Aparecen moscas
        tiempoSpawn -= dt;
        if (tiempoSpawn <= 0f && Vivas() < maxActivas && derrotadas + Vivas() < totalMoscas)
        {
            Aparecer();
            intervalo = Mathf.Max(intervaloMinimo, intervalo - reduccionIntervalo);
            tiempoSpawn = intervalo;
        }

        // Vuelan hacia la rosa
        float vel = velocidadInicial + incrementoVelocidad * derrotadas;

        for (int i = moscas.Count - 1; i >= 0; i--)
        {
            Mosca m = moscas[i];
            if (m.muriendo) continue;

            Vector2 pos = m.rt.anchoredPosition;
            Vector2 haciaRosa = PuntoRosa - pos;
            float dist = haciaRosa.magnitude;

            if (dist <= radioRosa)
            {
                Toque(m);
                continue;
            }

            Vector2 dir = haciaRosa / dist;
            Vector2 perp = new Vector2(-dir.y, dir.x);
            float zigzag = Mathf.Sin(Time.time * 7f + m.fase) * 70f;

            pos += (dir * vel + perp * zigzag) * dt;
            m.rt.anchoredPosition = pos;

            float aleteo = 0.55f + 0.45f * Mathf.Abs(Mathf.Sin(Time.time * 35f + m.fase));
            m.alaIzq.localScale = new Vector3(1f, aleteo, 1f);
            m.alaDer.localScale = new Vector3(1f, aleteo, 1f);
        }

        // ¿Ha terminado?
        if (toques >= toquesMaximos)
        {
            victoria = false;
            jugando = false;
        }
        else if (derrotadas >= totalMoscas)
        {
            victoria = true;
            jugando = false;
        }
    }

    int Vivas()
    {
        int n = 0;
        foreach (Mosca m in moscas)
            if (!m.muriendo) n++;
        return n;
    }

    void Aparecer()
    {
        char tecla = ElegirTecla();
        Vector2 pos;
        int lado = Random.Range(0, 3);

        if (lado == 0) pos = new Vector2(-1080f, Random.Range(-400f, 450f));
        else if (lado == 1) pos = new Vector2(1080f, Random.Range(-400f, 450f));
        else pos = new Vector2(Random.Range(-800f, 800f), 650f);

        moscas.Add(CrearMosca(tecla, pos));

        textoInfo.transform.SetAsLastSibling();
        textoMensaje.transform.SetAsLastSibling();
    }

    char ElegirTecla()
    {
        string letras = teclasPosibles.ToUpper();
        List<char> libres = new List<char>();

        foreach (char c in letras)
        {
            bool usada = false;
            foreach (Mosca m in moscas)
                if (!m.muriendo && m.tecla == c) usada = true;
            if (!usada) libres.Add(c);
        }

        if (libres.Count == 0)
            return letras[Random.Range(0, letras.Length)];

        return libres[Random.Range(0, libres.Count)];
    }

    void Intentar(char c)
    {
        Mosca mejor = null;
        float mejorDist = float.MaxValue;

        foreach (Mosca m in moscas)
        {
            if (m.muriendo || m.tecla != c) continue;

            float d = (m.rt.anchoredPosition - PuntoRosa).sqrMagnitude;
            if (d < mejorDist)
            {
                mejorDist = d;
                mejor = m;
            }
        }

        if (mejor != null) Derrotar(mejor);
    }

    void Derrotar(Mosca m)
    {
        m.muriendo = true;
        derrotadas++;
        RefrescarHud();
        StartCoroutine(Desvanecer(m));
    }

    IEnumerator Desvanecer(Mosca m)
    {
        Vector3 inicial = m.rt.localScale;
        float t = 0f;

        while (t < 1f)
        {
            if (m.rt == null) yield break;
            t += Time.deltaTime / 0.18f;
            m.rt.localScale = Vector3.Lerp(inicial, Vector3.zero, Mathf.Clamp01(t));
            yield return null;
        }

        if (m.rt != null) Destroy(m.rt.gameObject);
        moscas.Remove(m);
    }

    void Toque(Mosca m)
    {
        moscas.Remove(m);
        Destroy(m.rt.gameObject);

        toques++;
        ColorearRosa();
        RefrescarHud();
    }

    void ColorearRosa()
    {
        float k = Mathf.Clamp01(toques / (float)toquesMaximos);
        flor.color = Color.Lerp(RojoFlor, FlorMarchita, k);
        florCentro.color = Color.Lerp(RojoCentro, CentroMarchito, k);
    }

    void LimpiarMoscas()
    {
        foreach (Mosca m in moscas)
            if (m.rt != null) Destroy(m.rt.gameObject);
        moscas.Clear();
    }

    void Reiniciar()
    {
        LimpiarMoscas();
        derrotadas = 0;
        toques = 0;
        intervalo = intervaloInicial;
        tiempoSpawn = 0.8f;
        ColorearRosa();
        RefrescarHud();
    }

    // ---------- Textos ----------

    void RefrescarHud()
    {
        textoInfo.text = "Moscas derrotadas: " + derrotadas + "/" + totalMoscas +
                         "     ·     Salud de la rosa: " + (toquesMaximos - toques) + "/" + toquesMaximos;
    }

    void Mostrar(string texto)
    {
        textoMensaje.text = texto;
        textoMensaje.gameObject.SetActive(true);
    }

    void Ocultar()
    {
        textoMensaje.gameObject.SetActive(false);
    }
}