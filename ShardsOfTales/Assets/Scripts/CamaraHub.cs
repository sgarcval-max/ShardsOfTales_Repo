using UnityEngine;

public class CamaraHub : MonoBehaviour
{
    public enum Modo { Menu, AJuego, Juego, AMenu, AEstanteria, Estanteria }

    public Transform player;
    public PlayerHubMovement playerMovement;
    public GameObject panelMenu;
    public GameObject botonVolver;
    public Transform puntoEstanteria;

    public Vector3 centro = new Vector3(0f, 1.5f, 0f);
    public float radio = 11f;
    public float altura = 5f;
    public float velocidadGiro = 12f;

    public Vector3 offset = new Vector3(0f, 8f, -8f);
    public Vector3 rotacionJuego = new Vector3(45f, 0f, 0f);

    public float velocidadTransicion = 2f;
    public float velocidadSeguir = 5f;

    [HideInInspector] public bool escOcupado;

    private Modo modo = Modo.Menu;
    private float angulo;
    private Renderer[] rendersPlayer;

    void Start()
    {
        rendersPlayer = player.GetComponentsInChildren<Renderer>();

        playerMovement.enabled = false;
        botonVolver.SetActive(false);
        panelMenu.SetActive(true);

        Vector3 p = PosOrbita();
        transform.position = p;
        transform.rotation = RotOrbita(p);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !escOcupado &&
            (modo == Modo.AEstanteria || modo == Modo.Estanteria))
        {
            SalirEstanteria();
        }
    }

    void LateUpdate()
    {
        switch (modo)
        {
            case Modo.Menu:
                angulo += velocidadGiro * Time.deltaTime;
                Vector3 pm = PosOrbita();
                transform.position = pm;
                transform.rotation = RotOrbita(pm);
                break;

            case Modo.AJuego:
                Vector3 posJ = player.position + offset;
                Quaternion rotJ = Quaternion.Euler(rotacionJuego);
                Mover(posJ, rotJ, velocidadTransicion);
                if (Llego(posJ, rotJ))
                {
                    modo = Modo.Juego;
                    playerMovement.enabled = true;
                }
                break;

            case Modo.Juego:
                Mover(player.position + offset, Quaternion.Euler(rotacionJuego), velocidadSeguir);
                break;

            case Modo.AMenu:
                Vector3 posM = PosOrbita();
                Quaternion rotM = RotOrbita(posM);
                Mover(posM, rotM, velocidadTransicion);
                if (Llego(posM, rotM))
                {
                    modo = Modo.Menu;
                    panelMenu.SetActive(true);
                }
                break;

            case Modo.AEstanteria:
                Mover(puntoEstanteria.position, puntoEstanteria.rotation, velocidadTransicion);
                if (Llego(puntoEstanteria.position, puntoEstanteria.rotation))
                    modo = Modo.Estanteria;
                break;

            case Modo.Estanteria:
                transform.position = puntoEstanteria.position;
                transform.rotation = puntoEstanteria.rotation;
                break;
        }
    }

    Vector3 PosOrbita()
    {
        float rad = angulo * Mathf.Deg2Rad;
        return centro + new Vector3(Mathf.Sin(rad) * radio, altura, Mathf.Cos(rad) * radio);
    }

    Quaternion RotOrbita(Vector3 pos)
    {
        return Quaternion.LookRotation(centro - pos);
    }

    void Mover(Vector3 pos, Quaternion rot, float vel)
    {
        float t = 1f - Mathf.Exp(-vel * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, pos, t);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, t);
    }

    bool Llego(Vector3 pos, Quaternion rot)
    {
        return Vector3.Distance(transform.position, pos) < 0.15f &&
               Quaternion.Angle(transform.rotation, rot) < 1f;
    }

    void MostrarPlayer(bool visible)
    {
        foreach (Renderer r in rendersPlayer)
            r.enabled = visible;
    }

    public void Jugar()
    {
        if (modo != Modo.Menu) return;

        modo = Modo.AJuego;
        panelMenu.SetActive(false);
        botonVolver.SetActive(true);
    }

    public void Volver()
    {
        if (modo != Modo.Juego && modo != Modo.AJuego) return;

        modo = Modo.AMenu;
        playerMovement.enabled = false;
        botonVolver.SetActive(false);
    }

    public bool PuedeEntrarEstanteria()
    {
        return modo == Modo.Juego;
    }

    public bool EnVistaEstanteria()
    {
        return modo == Modo.Estanteria;
    }

    public void EntrarEstanteria()
    {
        if (modo != Modo.Juego) return;

        modo = Modo.AEstanteria;
        playerMovement.enabled = false;
        botonVolver.SetActive(false);
        MostrarPlayer(false);
    }

    public void SalirEstanteria()
    {
        if (modo != Modo.AEstanteria && modo != Modo.Estanteria) return;

        modo = Modo.AJuego;
        botonVolver.SetActive(true);
        MostrarPlayer(true);
    }

    public void Salir()
    {
        Debug.Log("Salir del juego");
        Application.Quit();
    }
}