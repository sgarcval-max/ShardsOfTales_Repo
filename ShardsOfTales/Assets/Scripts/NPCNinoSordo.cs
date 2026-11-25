using UnityEngine;
using TMPro;

public class NPCNinoSordo : MonoBehaviour
{
    public GameObject panelDialogo;
    public TMP_Text textoDialogo;
    public GameObject avisoInteractuar;
    public PlayerPlatformer2D player;

    public string[] lineasSinPapel = {
        "(El niño te mira y mueve los labios, pero no oye nada de lo que le dices.)",
        "(No hay forma de hablar con él. Quizá escribiendo...)",
        "(Necesitas papel y algo con que escribir. Tal vez en el pueblo.)"
    };

    public string[] lineasAyuda = {
        "(Le entregas el papel y el bolígrafo.)",
        "Niño: Gracias. Los demás niños siguieron el camino del bosque, hacia la montaña.",
        "(Ya sabes hacia dónde seguir.)"
    };

    public string[] lineasDespues = {
        "Niño: Sigue el camino hacia la montaña. ¡Suerte!"
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

    void Iniciar()
    {
        hablando = true;
        indice = 0;

        if (GameState.ninoSordoAyudado)
            actual = lineasDespues;
        else if (GameState.tienePapel && GameState.tieneBoligrafo)
            actual = lineasAyuda;
        else
            actual = lineasSinPapel;

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

        if (actual == lineasAyuda)
        {
            GameState.ninoSordoAyudado = true;
            Debug.Log("Niño sordo ayudado");
        }

        if (cerca) avisoInteractuar.SetActive(true);
    }
}
