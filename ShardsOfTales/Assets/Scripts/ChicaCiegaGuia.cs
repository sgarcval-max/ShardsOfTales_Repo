using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ChicaCiegaGuia : MonoBehaviour
{
    public Transform[] piedras;
    public TMP_Text textoMensaje;
    public TMP_Text textoNota;

    public float tamanoCelda = 2f;
    public float zOrillaSur = -1f;
    public float zMeta = 11f;
    public float limiteX = 6f;
    public float radioPiedra = 0.8f;
    public string escenaSiguiente = "SCN_Bosque_00";
    public bool forzarFlautaParaPruebas = true;

    private Vector3 posInicial;
    private bool ocupada;
    private bool terminado;
    private Coroutine rutinaNota;

    void Awake()
    {
        if (forzarFlautaParaPruebas) GameState.tieneFlauta = true;
    }

    void Start()
    {
        posInicial = transform.position;
        textoMensaje.gameObject.SetActive(false);
        textoNota.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!GameState.tieneFlauta || ocupada || terminado) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) Tocar("Do", Vector3.left);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) Tocar("Re", Vector3.forward);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) Tocar("Mi", Vector3.right);
    }

    void Tocar(string nombre, Vector3 direccion)
    {
        if (rutinaNota != null) StopCoroutine(rutinaNota);
        rutinaNota = StartCoroutine(MostrarNota(nombre));

        Vector3 destino = transform.position + direccion * tamanoCelda;
        if (Mathf.Abs(destino.x) > limiteX) return;

        StartCoroutine(Mover(destino));
    }

    IEnumerator Mover(Vector3 destino)
    {
        ocupada = true;

        Vector3 inicio = transform.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.3f;
            transform.position = Vector3.Lerp(inicio, destino, t);
            yield return null;
        }
        transform.position = destino;

        if (destino.z >= zMeta)
        {
            yield return StartCoroutine(Victoria());
            yield break;
        }

        bool segura = destino.z <= zOrillaSur || HayPiedra(destino);
        if (!segura)
            yield return StartCoroutine(Caida());

        ocupada = false;
    }

    bool HayPiedra(Vector3 pos)
    {
        Vector2 b = new Vector2(pos.x, pos.z);
        foreach (Transform p in piedras)
        {
            Vector2 a = new Vector2(p.position.x, p.position.z);
            if (Vector2.Distance(a, b) < radioPiedra) return true;
        }
        return false;
    }

    IEnumerator Caida()
    {
        textoMensaje.text = "¡Se ha caído al agua! Vuelve a empezar.";
        textoMensaje.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        transform.position = posInicial;
        textoMensaje.gameObject.SetActive(false);
    }

    IEnumerator Victoria()
    {
        terminado = true;
        GameState.ninaCiegaAyudada = true;
        textoMensaje.text = "¡La niña ha cruzado el río sana y salva!";
        textoMensaje.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(escenaSiguiente);
    }

    IEnumerator MostrarNota(string nombre)
    {
        textoNota.text = "Nota: " + nombre;
        textoNota.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        textoNota.gameObject.SetActive(false);
    }
}
