using UnityEngine;
using UnityEngine.Events;
using TMPro;

public enum AccionFinalConversacion
{
    Ninguna,
    DinDonHablado,
    BestiaHablada
}

public class NPCConversacion : MonoBehaviour
{
    public GameObject panelDialogo;
    public TMP_Text textoDialogo;
    public GameObject avisoInteractuar;
    public PlayerPlatformer2D player;

    public AccionFinalConversacion alTerminar = AccionFinalConversacion.Ninguna;
    public UnityEvent alAcabarConversacion;

    public string[] lineas =
    {
        "¡Bienvenido al castillo de la Bestia! Soy Din Don, el reloj.",
        "Tenemos un problema: Bella y la Bestia se han peleado, y así no se romperá el maleficio.",
        "Necesitamos tu ayuda. Ayuda a Lumiere en el comedor y a la señora Potts en la cocina.",
        "Cuando termines, podrás afrontar el último desafío. ¡El orden no importa!"
    };

    public string[] lineasDespues =
    {
        "Recuerda: ayuda a Lumiere en el comedor y a la señora Potts en la cocina."
    };

    private string[] actual;
    private int indice;
    private bool cerca;
    private bool hablando;

    void Start()
    {
        panelDialogo.SetActive(false);
        avisoInteractuar.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cerca = true;
            if (!hablando) avisoInteractuar.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cerca = false;
            avisoInteractuar.SetActive(false);
        }
    }

    void Update()
    {
        if (!hablando && cerca && Input.GetKeyDown(KeyCode.E))
            Iniciar();
        else if (hablando && Input.GetKeyDown(KeyCode.E))
            Siguiente();
    }

    bool YaHablado()
    {
        switch (alTerminar)
        {
            case AccionFinalConversacion.DinDonHablado: return GameState.dinDonHablado;
            case AccionFinalConversacion.BestiaHablada: return GameState.bestiaHablada;
            default: return false;
        }
    }

    void Iniciar()
    {
        hablando = true;
        indice = 0;
        actual = YaHablado() ? lineasDespues : lineas;

        textoDialogo.text = actual[0];
        panelDialogo.SetActive(true);
        avisoInteractuar.SetActive(false);
        player.enabled = false;
    }

    void Siguiente()
    {
        indice++;
        if (indice >= actual.Length)
            Terminar();
        else
            textoDialogo.text = actual[indice];
    }

    void Terminar()
    {
        hablando = false;
        panelDialogo.SetActive(false);
        player.enabled = true;

        if (actual == lineas)
        {
            if (alTerminar == AccionFinalConversacion.DinDonHablado)
                GameState.dinDonHablado = true;
            else if (alTerminar == AccionFinalConversacion.BestiaHablada)
                GameState.bestiaHablada = true;
        }

        if (cerca) avisoInteractuar.SetActive(true);

        if (alAcabarConversacion != null)
            alAcabarConversacion.Invoke();
    }
}