using UnityEngine;
using TMPro;
using System.Collections;

public enum CondicionSalida
{
    Ninguna,
    RatonesCapturados,
    NinoSordoAyudado
}

public class SalidaEscena : MonoBehaviour
{
    public TMP_Text textoMensaje;
    public string escenaDestino;
    public bool adelante = true;
    public CondicionSalida condicion = CondicionSalida.Ninguna;
    public string mensajeAviso = "Pulsa E para salir";
    public string mensajeBloqueado = "No puedes salir todavía";

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
            Mostrar(mensajeAviso);
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
        if (!cerca || !Input.GetKeyDown(KeyCode.E)) return;

        if (CondicionCumplida())
        {
            textoMensaje.gameObject.SetActive(false);
            TransicionPagina.IrA(escenaDestino, adelante);
        }
        else
        {
            Mostrar(mensajeBloqueado);
            if (rutina != null) StopCoroutine(rutina);
            rutina = StartCoroutine(Ocultar());
        }
    }

    bool CondicionCumplida()
    {
        if (condicion == CondicionSalida.RatonesCapturados)
            return GameState.ratonesCompletados;
        if (condicion == CondicionSalida.NinoSordoAyudado)
            return GameState.ninoSordoAyudado;
        return true;
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