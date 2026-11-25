using UnityEngine;
using TMPro;
using System.Collections;

public enum TipoObjeto
{
    Papel,
    Boligrafo
}

public class ObjetoRecogible : MonoBehaviour
{
    public TMP_Text textoMensaje;
    public TipoObjeto tipo;

    private bool cerca;
    private bool recogido;
    private Coroutine rutina;

    void Start()
    {
        textoMensaje.gameObject.SetActive(false);

        if (YaLoTengo())
            gameObject.SetActive(false);
    }

    bool YaLoTengo()
    {
        return tipo == TipoObjeto.Papel ? GameState.tienePapel : GameState.tieneBoligrafo;
    }

    string Nombre()
    {
        return tipo == TipoObjeto.Papel ? "el papel" : "el bolígrafo";
    }

    void OnTriggerEnter(Collider other)
    {
        if (recogido || !other.CompareTag("Player")) return;

        cerca = true;
        textoMensaje.text = "Pulsa E para coger " + Nombre();
        textoMensaje.gameObject.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (recogido || !other.CompareTag("Player")) return;

        cerca = false;
        textoMensaje.gameObject.SetActive(false);
    }

    void Update()
    {
        if (cerca && !recogido && Input.GetKeyDown(KeyCode.E))
            Recoger();
    }

    void Recoger()
    {
        recogido = true;
        cerca = false;

        if (tipo == TipoObjeto.Papel)
            GameState.tienePapel = true;
        else
            GameState.tieneBoligrafo = true;

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        GetComponent<Collider>().enabled = false;

        string mensaje = "Has cogido " + Nombre();
        if (GameState.tienePapel && GameState.tieneBoligrafo)
            mensaje += ". Ya puedes escribirle al niño sordo.";

        rutina = StartCoroutine(MostrarMensaje(mensaje));
    }

    IEnumerator MostrarMensaje(string mensaje)
    {
        textoMensaje.text = mensaje;
        textoMensaje.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        textoMensaje.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}