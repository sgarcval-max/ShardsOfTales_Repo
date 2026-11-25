using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PuertaSalida : MonoBehaviour
{
    public TMP_Text textoMensaje;
    public string escenaDestino = "SCN_Pueblo_00";

    private bool cerca;
    private Coroutine rutina;

    void Start()
    {
        textoMensaje.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cerca = true;
            Mostrar("Pulsa E para abrir la puerta");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cerca = false;
            if (rutina != null) StopCoroutine(rutina);
            textoMensaje.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (cerca && Input.GetKeyDown(KeyCode.E))
        {
            if (GameState.tienePartitura)
            {
                SceneManager.LoadScene(escenaDestino);
            }
            else
            {
                Mostrar("La puerta está cerrada. Falta la partitura.");
                if (rutina != null) StopCoroutine(rutina);
                rutina = StartCoroutine(Ocultar());
            }
        }
    }

    void Mostrar(string texto)
    {
        textoMensaje.text = texto;
        textoMensaje.gameObject.SetActive(true);
    }

    IEnumerator Ocultar()
    {
        yield return new WaitForSeconds(2.5f);
        textoMensaje.gameObject.SetActive(false);
    }
}
