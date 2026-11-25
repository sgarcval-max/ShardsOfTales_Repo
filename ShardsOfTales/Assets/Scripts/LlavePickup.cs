using UnityEngine;
using TMPro;
using System.Collections;

public class LlavePickup : MonoBehaviour
{
    public TMP_Text textoMensaje;

    private bool recogida;

    void OnTriggerEnter(Collider other)
    {
        if (recogida || !other.CompareTag("Player")) return;

        recogida = true;
        GameState.tieneLlave = true;

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        StartCoroutine(MostrarMensaje());
    }

    IEnumerator MostrarMensaje()
    {
        textoMensaje.text = "Has cogido la llave";
        textoMensaje.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        textoMensaje.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
