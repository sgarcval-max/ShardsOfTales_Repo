using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MinijuegoVelas : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text textoInfo;
    public TMP_Text textoMensaje;
    public PlayerPlatformer2D player;

    public int velasTotales = 6;
    public int aciertosPorVela = 3;
    public int fallosPorVela = 3;

    public float diametroInicial = 480f;
    public float diametroObjetivo = 180f;
    public float reduccionPorAcierto = 14f;
    public float tolerancia = 28f;
    public float reduccionTolerancia = 5f;
    public float duracionAnillo = 1.8f;

    public Vector2 centroRueda = new Vector2(0f, -200f);
    public Vector2 centroCandelabro = new Vector2(0f, 170f);

    private RectTransform contenedor;
    private Image[] llamas;
    private Image anilloObjetivo;
    private Image anilloMovil;

    private bool activo;
    private bool resultadoIntento;
    private int velasEncendidas;
    private int aciertos;
    private int fallos;
    private bool ultimaOportunidadUsada;

    private static readonly Color Verde = new Color(0.3f, 1f, 0.4f);
    private static readonly Color Rojo = new Color(1f, 0.3f, 0.3f);

    void Start()
    {
        contenedor = panel.GetComponent<RectTransform>();

        Sprite circulo = CrearSprite(256, 0f);
        Sprite anillo = CrearSprite(256, 0.9f);

        Color metal = new Color(0.55f, 0.55f, 0.6f);
        CrearImagen("Barra", centroCandelabro + new Vector2(0f, -80f), new Vector2(620f, 26f), metal, null);
        CrearImagen("Pie", centroCandelabro + new Vector2(0f, -125f), new Vector2(24f, 70f), metal, null);

        llamas = new Image[velasTotales];
        for (int i = 0; i < velasTotales; i++)
        {
            float x = (i - (velasTotales - 1) * 0.5f) * 90f;

            CrearImagen("Vela_" + i, centroCandelabro + new Vector2(x, 0f), new Vector2(46f, 130f),
                new Color(0.95f, 0.88f, 0.75f), null);
            CrearImagen("Mecha_" + i, centroCandelabro + new Vector2(x, 72f), new Vector2(6f, 14f),
                new Color(0.2f, 0.15f, 0.1f), null);
            llamas[i] = CrearImagen("Llama_" + i, centroCandelabro + new Vector2(x, 100f), new Vector2(34f, 46f),
                new Color(1f, 0.65f, 0.1f), circulo);
        }

        anilloObjetivo = CrearImagen("Anillo_Objetivo", centroRueda,
            new Vector2(diametroObjetivo, diametroObjetivo), Color.white, anillo);
        anilloMovil = CrearImagen("Anillo_Movil", centroRueda,
            new Vector2(diametroInicial, diametroInicial), new Color(1f, 0.9f, 0.4f), anillo);
        anilloObjetivo.gameObject.SetActive(false);
        anilloMovil.gameObject.SetActive(false);

        textoInfo.transform.SetAsLastSibling();
        textoMensaje.transform.SetAsLastSibling();
        textoMensaje.gameObject.SetActive(false);

        panel.SetActive(false);
    }

    // ---------- Dibujo ----------

    static Sprite CrearSprite(int tam, float radioInterno)
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
                if (radioInterno > 0f)
                    alpha = Mathf.Min(alpha, Mathf.Clamp01((d - radioInterno) * c));
                px[y * tam + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(px);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, tam, tam), new Vector2(0.5f, 0.5f), 100f);
    }

    Image CrearImagen(string nombre, Vector2 pos, Vector2 tam, Color color, Sprite sprite)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform), typeof(Image));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(contenedor, false);
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

    void Refrescar()
    {
        for (int i = 0; i < llamas.Length; i++)
            llamas[i].enabled = i < velasEncendidas;

        string t = "Vela " + Mathf.Min(velasEncendidas + 1, velasTotales) + " de " + velasTotales +
                   "   ·   Aciertos: " + aciertos + "/" + aciertosPorVela +
                   "   ·   Fallos: " + fallos + "/" + fallosPorVela;
        if (velasEncendidas == 0 && ultimaOportunidadUsada)
            t += "   ·   ¡Última oportunidad!";
        textoInfo.text = t;
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

    void Reiniciar()
    {
        velasEncendidas = 1;
        aciertos = 0;
        fallos = 0;
        ultimaOportunidadUsada = false;
    }

    // ---------- Juego ----------

    public void Iniciar()
    {
        if (activo) return;

        if (GameState.velasCompletadas)
            StartCoroutine(YaCompletado());
        else
            StartCoroutine(Partida());
    }

    IEnumerator YaCompletado()
    {
        activo = true;
        player.enabled = false;

        velasEncendidas = velasTotales;
        aciertos = 0;
        fallos = 0;
        panel.SetActive(true);
        Refrescar();

        Mostrar("Las " + velasTotales + " velas ya están encendidas. ¡Gracias!");
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

        Reiniciar();
        panel.SetActive(true);
        Refrescar();

        Mostrar("Pulsa ESPACIO (o haz clic) cuando el círculo que se encoge coincida con el círculo fijo.");
        yield return new WaitForSeconds(3f);
        Ocultar();

        while (velasEncendidas < velasTotales)
        {
            yield return Intento();

            bool perdido = false;

            if (resultadoIntento)
            {
                aciertos++;
                if (aciertos >= aciertosPorVela)
                {
                    velasEncendidas++;
                    aciertos = 0;
                    fallos = 0;
                    Refrescar();

                    if (velasEncendidas < velasTotales)
                    {
                        Mostrar("¡Vela encendida!");
                        yield return new WaitForSeconds(0.9f);
                        Ocultar();
                    }
                }
            }
            else
            {
                fallos++;
                if (fallos >= fallosPorVela)
                {
                    aciertos = 0;
                    fallos = 0;

                    if (velasEncendidas == 0)
                    {
                        perdido = true;
                    }
                    else
                    {
                        velasEncendidas--;
                        Refrescar();

                        if (velasEncendidas == 0)
                        {
                            if (ultimaOportunidadUsada)
                            {
                                perdido = true;
                            }
                            else
                            {
                                ultimaOportunidadUsada = true;
                                Refrescar();
                                Mostrar("Se han apagado todas las velas. ¡Última oportunidad!");
                                yield return new WaitForSeconds(2.2f);
                                Ocultar();
                            }
                        }
                        else
                        {
                            Mostrar("Se ha apagado una vela. ¡Hay que volver a encenderla!");
                            yield return new WaitForSeconds(1.6f);
                            Ocultar();
                        }
                    }
                }
            }

            Refrescar();

            if (perdido)
            {
                Mostrar("¡Game over! Las velas se han apagado.\nVuelve a intentarlo.");
                yield return new WaitForSeconds(2.5f);
                Ocultar();
                Reiniciar();
                Refrescar();
            }
        }

        GameState.velasCompletadas = true;
        Mostrar("¡Las " + velasTotales + " velas están encendidas!\nEl comedor luce de lo más romántico.");
        yield return new WaitForSeconds(3f);
        Ocultar();

        panel.SetActive(false);
        player.enabled = true;
        activo = false;
    }

    IEnumerator Intento()
    {
        float objetivo = Mathf.Max(60f, diametroObjetivo - reduccionPorAcierto * aciertos);
        float tol = Mathf.Max(12f, tolerancia - reduccionTolerancia * aciertos);

        anilloObjetivo.rectTransform.sizeDelta = new Vector2(objetivo, objetivo);
        anilloObjetivo.color = Color.white;
        anilloMovil.color = new Color(1f, 0.9f, 0.4f);
        anilloObjetivo.gameObject.SetActive(true);
        anilloMovil.gameObject.SetActive(true);

        resultadoIntento = false;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duracionAnillo;
            float d = Mathf.Lerp(diametroInicial, 0f, Mathf.Clamp01(t));
            anilloMovil.rectTransform.sizeDelta = new Vector2(d, d);

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                resultadoIntento = Mathf.Abs(d - objetivo) <= tol;
                break;
            }

            if (d < objetivo - tol) break;

            yield return null;
        }

        Color feedback = resultadoIntento ? Verde : Rojo;
        anilloObjetivo.color = feedback;
        anilloMovil.color = feedback;
        yield return new WaitForSeconds(0.5f);

        anilloObjetivo.gameObject.SetActive(false);
        anilloMovil.gameObject.SetActive(false);
    }
}