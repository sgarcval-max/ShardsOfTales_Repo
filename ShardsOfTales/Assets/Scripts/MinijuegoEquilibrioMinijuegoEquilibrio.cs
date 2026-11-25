using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MinijuegoEquilibrio : MonoBehaviour
{
    public RectTransform barra;
    public RectTransform indicadorViento;
    public RectTransform indicadorEquilibrio;
    public Image imagenEquilibrio;
    public TMP_Text textoMensaje;

    public Transform nino;
    public float xInicio = -7f;
    public float xFin = 7f;

    public float velocidadJugador = 450f;
    public float velocidadViento = 0.6f;
    public float duracionCruce = 15f;
    public float tiempoParaCaer = 2f;
    public float inclinacionMax = 30f;
    public string escenaSiguiente = "SCN_Bosque_00";

    public Color colorBien = new Color(0.3f, 1f, 0.3f);
    public Color colorMal = new Color(1f, 0.25f, 0.25f);

    private float seed;
    private float fase;
    private float progreso;
    private float peligro;
    private float lado = 1f;
    private bool terminado;
    private bool reiniciando;

    void Start()
    {
        seed = Random.Range(0f, 100f);
        textoMensaje.gameObject.SetActive(false);
        Reiniciar();
    }

    float MaxX(RectTransform ind)
    {
        return barra.rect.width / 2f - ind.rect.width / 2f;
    }

    void Reiniciar()
    {
        fase = 0f;
        progreso = 0f;
        peligro = 0f;
        indicadorEquilibrio.anchoredPosition = Vector2.zero;
        nino.position = new Vector3(xInicio, nino.position.y, nino.position.z);
        nino.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (terminado || reiniciando) return;

        // Viento: se mueve solo y cada vez más rápido
        fase += Time.deltaTime * velocidadViento * (1f + progreso * 0.8f);
        float ruido = Mathf.PerlinNoise(seed + fase, 0f);
        float normal = Mathf.Clamp((ruido - 0.5f) * 2f * 1.8f, -1f, 1f);
        float posViento = normal * MaxX(indicadorViento);
        indicadorViento.anchoredPosition = new Vector2(posViento, 0f);

        // Jugador: mueve su marcador con A y D
        float input = Input.GetAxisRaw("Horizontal");
        float x = indicadorEquilibrio.anchoredPosition.x + input * velocidadJugador * Time.deltaTime;
        float maxEq = MaxX(indicadorEquilibrio);
        x = Mathf.Clamp(x, -maxEq, maxEq);
        indicadorEquilibrio.anchoredPosition = new Vector2(x, 0f);

        // ¿Está dentro de la zona del viento?
        float distancia = Mathf.Abs(x - posViento);
        bool dentro = distancia <= indicadorViento.rect.width / 2f;
        imagenEquilibrio.color = dentro ? colorBien : colorMal;

        if (x != posViento) lado = Mathf.Sign(x - posViento);

        if (dentro)
        {
            progreso += Time.deltaTime / duracionCruce;
            peligro = Mathf.Max(0f, peligro - Time.deltaTime);
        }
        else
        {
            peligro += Time.deltaTime;
        }

        // El niño avanza e se inclina
        float nx = Mathf.Lerp(xInicio, xFin, Mathf.Clamp01(progreso));
        nino.position = new Vector3(nx, nino.position.y, nino.position.z);
        float inclinacion = Mathf.Clamp01(peligro / tiempoParaCaer) * inclinacionMax * lado;
        nino.rotation = Quaternion.Euler(0f, 0f, -inclinacion);

        if (peligro >= tiempoParaCaer)
            StartCoroutine(Caida());
        else if (progreso >= 1f)
            StartCoroutine(Victoria());
    }

    IEnumerator Caida()
    {
        reiniciando = true;
        nino.rotation = Quaternion.Euler(0f, 0f, -90f * lado);
        textoMensaje.text = "¡El niño ha perdido el equilibrio! Inténtalo de nuevo.";
        textoMensaje.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.8f);
        textoMensaje.gameObject.SetActive(false);
        Reiniciar();
        reiniciando = false;
    }

    IEnumerator Victoria()
    {
        terminado = true;
        GameState.ninoCojoAyudado = true;
        textoMensaje.text = "¡El niño ha cruzado el tronco! La cueva está justo delante.";
        textoMensaje.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(escenaSiguiente);
    }
}
