using UnityEngine;

public class PuzzleNotas : MonoBehaviour
{
    public GameObject[] plataformas;
    public int[] secuencia = { 3, 5, 1 };
    public Color colorApagado = new Color(0.25f, 0.25f, 0.25f);
    public Color colorEncendido = new Color(1f, 0.85f, 0.2f);

    private int siguiente;
    private bool jugadorDentro;

    void Start()
    {
        foreach (GameObject p in plataformas)
            Poner(p, false);
    }

    void OnEnable()
    {
        FlautaPlayer.OnNotaTocada += NotaTocada;
    }

    void OnDisable()
    {
        FlautaPlayer.OnNotaTocada -= NotaTocada;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) jugadorDentro = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) jugadorDentro = false;
    }

    void NotaTocada(int nota)
    {
        if (!jugadorDentro) return;
        if (siguiente >= plataformas.Length || siguiente >= secuencia.Length) return;

        if (nota == secuencia[siguiente])
        {
            Poner(plataformas[siguiente], true);
            siguiente++;
        }
    }

    void Poner(GameObject p, bool encendida)
    {
        p.GetComponent<Renderer>().material.color = encendida ? colorEncendido : colorApagado;
        p.GetComponent<Collider>().enabled = encendida;
    }
}