using UnityEngine;
using TMPro;
using System.Collections;

public class CofreInteract : MonoBehaviour
{
    public TMP_Text textoMensaje;
    public SpriteRenderer spriteCofre;
    public Color colorAbierto = new Color(0.6f, 0.4f, 0.1f);

    private bool cerca;
    private bool abierto;
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
            if (!abierto) Mostrar("Pulsa E para abrir el cofre");
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
        if (cerca && !abierto && Input.GetKeyDown(KeyCode.E))
        {
            if (GameState.tieneLlave)
            {
                abierto = true;
                GameState.tienePartitura = true;
                spriteCofre.color = colorAbierto;
                MostrarTemporal("Has conseguido la partitura");
            }
            else
            {
                MostrarTemporal("El cofre está cerrado. Necesitas una llave.");
            }
        }
    }

    void Mostrar(string texto)
    {
        if (rutina != null) StopCoroutine(rutina);
        textoMensaje.text = texto;
        textoMensaje.gameObject.SetActive(true);
    }

    void MostrarTemporal(string texto)
    {
        Mostrar(texto);
        rutina = StartCoroutine(Ocultar());
    }

    IEnumerator Ocultar()
    {
        yield return new WaitForSeconds(2.5f);
        textoMensaje.gameObject.SetActive(false);
        rutina = null;
    }
}
