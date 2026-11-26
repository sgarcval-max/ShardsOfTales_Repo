using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroEscena : MonoBehaviour
{
    public CanvasGroup logo;
    public string escenaSiguiente = "SCN_Hub_00";
    public float esperaInicial = 0.8f;
    public float duracionFadeIn = 1.5f;
    public float tiempoVisible = 2f;
    public float duracionFadeOut = 1.2f;
    public float esperaFinal = 0.6f;
    public bool permitirSaltar = true;

    private bool saliendo;

    void Start()
    {
        logo.alpha = 0f;
        StartCoroutine(Secuencia());
    }

    void Update()
    {
        if (!permitirSaltar || saliendo) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
        {
            Ir();
        }
    }

    IEnumerator Secuencia()
    {
        yield return new WaitForSeconds(esperaInicial);
        yield return Fundir(0f, 1f, duracionFadeIn);
        yield return new WaitForSeconds(tiempoVisible);
        yield return Fundir(1f, 0f, duracionFadeOut);
        yield return new WaitForSeconds(esperaFinal);
        Ir();
    }

    IEnumerator Fundir(float de, float a, float duracion)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            logo.alpha = Mathf.Lerp(de, a, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t)));
            yield return null;
        }
        logo.alpha = a;
    }

    void Ir()
    {
        if (saliendo) return;
        saliendo = true;
        SceneManager.LoadScene(escenaSiguiente);
    }
}
