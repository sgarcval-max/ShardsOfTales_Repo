using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class EstanteriaLibros : MonoBehaviour
{
    public CamaraHub camara;
    public Transform puntoLibro;
    public TMP_Text textoLibroAbierto;

    private Camera cam;
    private LibroEstanteria actual;
    private LibroEstanteria abierto;
    private bool animando;
    private bool cargando;

    void Start()
    {
        cam = camara.GetComponent<Camera>();
        textoLibroAbierto.gameObject.SetActive(false);
    }

    void Update()
    {
        if (cargando) return;

        if (abierto != null)
        {
            if (animando) return;

            if (Input.GetKeyDown(KeyCode.Escape))
                StartCoroutine(CerrarLibro());
            else if (Input.GetMouseButtonDown(0) && BuscarLibro() == abierto)
                StartCoroutine(CargarNivel(abierto));

            return;
        }

        LibroEstanteria bajoRaton = null;

        if (camara.EnVistaEstanteria())
            bajoRaton = BuscarLibro();

        if (bajoRaton != actual)
        {
            if (actual != null) actual.resaltado = false;
            actual = bajoRaton;
            if (actual != null) actual.resaltado = true;
        }

        if (actual != null && Input.GetMouseButtonDown(0))
            StartCoroutine(AbrirLibro(actual));
    }

    IEnumerator AbrirLibro(LibroEstanteria libro)
    {
        animando = true;
        abierto = libro;
        camara.escOcupado = true;

        libro.resaltado = false;
        actual = null;

        yield return StartCoroutine(libro.Abrir(puntoLibro.position));

        textoLibroAbierto.text = libro.nombreNivel +
            "\n<size=36>Haz clic en el libro para jugar · ESC para cerrar</size>";
        textoLibroAbierto.gameObject.SetActive(true);
        animando = false;
    }

    IEnumerator CerrarLibro()
    {
        animando = true;
        textoLibroAbierto.gameObject.SetActive(false);

        yield return StartCoroutine(abierto.Cerrar());

        abierto = null;
        camara.escOcupado = false;
        animando = false;
    }

    IEnumerator CargarNivel(LibroEstanteria libro)
    {
        cargando = true;
        textoLibroAbierto.text = "Cargando...";
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(libro.escenaNivel);
    }

    LibroEstanteria BuscarLibro()
    {
        Ray rayo = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] golpes = Physics.RaycastAll(rayo, 50f, ~0, QueryTriggerInteraction.Ignore);

        LibroEstanteria mejor = null;
        float mejorDistancia = float.MaxValue;

        foreach (RaycastHit g in golpes)
        {
            LibroEstanteria libro = g.collider.GetComponentInParent<LibroEstanteria>();
            if (libro != null && g.distance < mejorDistancia)
            {
                mejorDistancia = g.distance;
                mejor = libro;
            }
        }

        return mejor;
    }
}