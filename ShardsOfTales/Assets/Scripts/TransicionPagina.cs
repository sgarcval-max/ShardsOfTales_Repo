using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransicionPagina : MonoBehaviour
{
    static TransicionPagina instancia;

    const float DuracionCubrir = 0.6f;
    const float DuracionDescubrir = 0.6f;
    const float AnchoSombra = 90f;
    static readonly Color ColorPagina = new Color(0.96f, 0.92f, 0.8f);
    static readonly Color ColorSombra = new Color(0.2f, 0.12f, 0.05f, 0.35f);

    private RectTransform pagina;
    private RectTransform sombra;
    private Image imgPagina;
    private bool ocupado;

    public static void IrA(string escena, bool adelante = true)
    {
        if (instancia == null)
        {
            GameObject go = new GameObject("TransicionPagina");
            instancia = go.AddComponent<TransicionPagina>();
        }

        instancia.Iniciar(escena, adelante);
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        GameObject canvasGO = new GameObject("Canvas_Pagina",
            typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster));
        canvasGO.transform.SetParent(transform, false);

        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        GameObject pagGO = new GameObject("Pagina", typeof(RectTransform), typeof(Image));
        pagGO.transform.SetParent(canvasGO.transform, false);
        pagina = pagGO.GetComponent<RectTransform>();
        imgPagina = pagGO.GetComponent<Image>();
        imgPagina.color = ColorPagina;
        imgPagina.raycastTarget = false;

        GameObject sombraGO = new GameObject("Sombra", typeof(RectTransform), typeof(Image));
        sombraGO.transform.SetParent(pagGO.transform, false);
        sombra = sombraGO.GetComponent<RectTransform>();
        Image imgSombra = sombraGO.GetComponent<Image>();
        imgSombra.color = ColorSombra;
        imgSombra.raycastTarget = false;

        pagGO.SetActive(false);
    }

    void Iniciar(string escena, bool adelante)
    {
        if (ocupado) return;
        ocupado = true;
        StartCoroutine(Rutina(escena, adelante));
    }

    IEnumerator Rutina(string escena, bool adelante)
    {
        imgPagina.raycastTarget = true;

        yield return Barrer(true, adelante, DuracionCubrir);
        yield return new WaitForSecondsRealtime(0.1f);

        AsyncOperation op = SceneManager.LoadSceneAsync(escena);
        if (op != null)
        {
            while (!op.isDone) yield return null;
            yield return null;
        }

        yield return Barrer(false, adelante, DuracionDescubrir);

        pagina.gameObject.SetActive(false);
        imgPagina.raycastTarget = false;
        ocupado = false;
    }

    IEnumerator Barrer(bool cubrir, bool adelante, float duracion)
    {
        pagina.gameObject.SetActive(true);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duracion;
            Aplicar(Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t)), cubrir, adelante);
            yield return null;
        }

        Aplicar(1f, cubrir, adelante);
    }

    void Aplicar(float k, bool cubrir, bool adelante)
    {
        float a;
        float b;
        bool bordeIzquierdo;

        if (adelante)
        {
            if (cubrir) { a = 1f - k; b = 1f; bordeIzquierdo = true; }
            else { a = 0f; b = 1f - k; bordeIzquierdo = false; }
        }
        else
        {
            if (cubrir) { a = 0f; b = k; bordeIzquierdo = false; }
            else { a = k; b = 1f; bordeIzquierdo = true; }
        }

        pagina.anchorMin = new Vector2(a, 0f);
        pagina.anchorMax = new Vector2(b, 1f);
        pagina.offsetMin = Vector2.zero;
        pagina.offsetMax = Vector2.zero;

        if (bordeIzquierdo)
        {
            sombra.anchorMin = new Vector2(0f, 0f);
            sombra.anchorMax = new Vector2(0f, 1f);
            sombra.pivot = new Vector2(1f, 0.5f);
        }
        else
        {
            sombra.anchorMin = new Vector2(1f, 0f);
            sombra.anchorMax = new Vector2(1f, 1f);
            sombra.pivot = new Vector2(0f, 0.5f);
        }

        sombra.sizeDelta = new Vector2(AnchoSombra, 0f);
        sombra.anchoredPosition = Vector2.zero;
        sombra.gameObject.SetActive(b - a > 0.001f);
    }
}
