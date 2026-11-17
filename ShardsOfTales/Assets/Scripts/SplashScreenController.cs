using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreenController : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image logoImage;

    [Header("Tiempos (en segundos)")]
    [SerializeField] private float fadeInDuration = 1.0f;
    [SerializeField] private float displayDuration = 4.5f;
    [SerializeField] private float fadeOutDuration = 1.0f;

    [Header("Siguiente Escena")]
    [SerializeField] private string hubSceneName = "SCN_Hub_01";

    private void Start()
    {
        if (logoImage != null)
        {
            SetLogoAlpha(0f);
        }

        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        // Pequeña pausa inicial en negro
        yield return new WaitForSeconds(0.4f);

        // 1. Fade In del logo
        yield return StartCoroutine(FadeLogo(0f, 1f, fadeInDuration));

        // 2. Mantener logo en pantalla 4-5 segundos
        yield return new WaitForSeconds(displayDuration);

        // 3. Fade Out del logo a negro
        yield return StartCoroutine(FadeLogo(1f, 0f, fadeOutDuration));

        // Pequeña pausa en negro antes de cargar el Hub
        yield return new WaitForSeconds(0.3f);

        SceneManager.LoadScene(hubSceneName);
    }

    private IEnumerator FadeLogo(float startAlpha, float endAlpha, float duration)
    {
        if (logoImage == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(elapsed / duration));
            SetLogoAlpha(alpha);
            yield return null;
        }

        SetLogoAlpha(endAlpha);
    }

    private void SetLogoAlpha(float alpha)
    {
        Color c = logoImage.color;
        c.a = alpha;
        logoImage.color = c;
    }
}
