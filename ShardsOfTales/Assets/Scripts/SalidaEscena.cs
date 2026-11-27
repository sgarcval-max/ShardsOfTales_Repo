using UnityEngine;
using TMPro;
using System.Collections;

public enum CondicionSalida
{
    Ninguna,
    RatonesCapturados,
    NinoSordoAyudado,
    DinDonHablado,
    BestiaHablada
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

        if (!CondicionCumplida())
        {
            MostrarTemporal(mensajeBloqueado);
        }
        else if (string.IsNullOrEmpty(escenaDestino))
        {
            MostrarTemporal("Esta sala aún no está construida.");
        }
        else
        {
            textoMensaje.gameObject.SetActive(false);
            TransicionPagina.IrA(escenaDestino, adelante);
        }
    }

    bool CondicionCumplida()
    {
        switch (condicion)
        {
            case CondicionSalida.RatonesCapturados: return GameState.ratonesCompletados;
            case CondicionSalida.NinoSordoAyudado: return GameState.ninoSordoAyudado;
            case CondicionSalida.DinDonHablado: return GameState.dinDonHablado;
            case CondicionSalida.BestiaHablada: return GameState.bestiaHablada;
            default: return true;
        }
    }

    void Mostrar(string texto)
    {
        textoMensaje.text = texto;
        textoMensaje.gameObject.SetActive(true);
    }

    void MostrarTemporal(string texto)
    {
        Mostrar(texto);
        if (rutina != null) StopCoroutine(rutina);
        rutina = StartCoroutine(Ocultar());
    }

    IEnumerator Ocultar()
    {
        yield return new WaitForSeconds(2.5f);
        textoMensaje.gameObject.SetActive(false);
    }
}