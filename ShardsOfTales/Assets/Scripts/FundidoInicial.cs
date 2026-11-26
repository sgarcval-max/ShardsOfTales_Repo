using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FundidoInicial : MonoBehaviour
{
    public float espera = 0.3f;
    public float duracion = 1.5f;

    IEnumerator Start()
    {
        Image img = GetComponent<Image>();
        img.raycastTarget = false;

        Color c = Color.black;
        c.a = 1f;
        img.color = c;

        yield return new WaitForSeconds(espera);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            c.a = 1f - Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
            img.color = c;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
