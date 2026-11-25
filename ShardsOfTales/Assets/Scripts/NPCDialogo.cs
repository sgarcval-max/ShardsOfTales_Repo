using UnityEngine;
using TMPro;

public class NPCDialogo : MonoBehaviour
{
    public GameObject panelDialogo;
    public TMP_Text textoDialogo;
    public GameObject avisoInteractuar;
    public PlayerPlatformer2D player;

    public string[] lineas = {
        "Ay... estoy enfermo y no puedo salir de la torre.",
        "Toma mi flauta, la vas a necesitar.",
        "Mi partitura está en el cofre del primer piso, pero está cerrado.",
        "Busca la llave en el segundo piso.",
        "(Has recibido la flauta)"
    };

    public string[] lineasDespues = {
        "Busca la llave en el segundo piso y abre el cofre del primer piso."
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
        actual = GameState.tieneFlauta ? lineasDespues : lineas;
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
            GameState.tieneFlauta = true;
            Debug.Log("Flauta obtenida");
        }

        if (cerca) avisoInteractuar.SetActive(true);
    }
}
