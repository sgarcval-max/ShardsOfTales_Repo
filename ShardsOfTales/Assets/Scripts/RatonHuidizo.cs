using UnityEngine;
using System.Collections;

public class RatonHuidizo : MonoBehaviour
{
    enum Estado { Libre, Siguiendo, Desapareciendo }

    public Transform player;
    public RatonesManager manager;
    public SpriteRenderer sprite;

    public float velocidadMin = 4f;
    public float velocidadMax = 9f;
    public float radioHuida = 6f;
    public float radioCaptura = 1.3f;
    public float radioMelodia = 14f;
    public float duracionQuieto = 8f;
    public float limiteIzq = -19f;
    public float limiteDer = 19f;

    public float alturaSaltoHuida = 0.25f;
    public float frecuenciaSaltoHuida = 16f;
    public float radioEsquina = 2.5f;
    public float radioJugadorEsquina = 3.5f;
    public float duracionEsquive = 0.6f;
    public float velocidadEsquive = 11f;
    public float esperaEntreEsquives = 1.2f;

    public float separacionSeguir = 1.1f;
    public float velocidadSeguir = 6f;
    public float alturaSuelo = 0.2f;
    public float alturaSalto = 0.2f;
    public float frecuenciaSalto = 14f;

    public Color colorNormal = new Color(0.55f, 0.55f, 0.55f);
    public Color colorQuieto = new Color(0.4f, 0.7f, 1f);

    private Estado estado = Estado.Libre;
    private float tiempoQuieto;
    private int indiceSeguir;
    private float fase;
    private float tiempoEsquive;
    private float dirEsquive;
    private float cooldownEsquive;

    void Start()
    {
        fase = Random.Range(0f, 6.28f);

        if (GameState.ratonesCompletados)
        {
            gameObject.SetActive(false);
            return;
        }

        sprite.color = colorNormal;
    }

    void OnEnable()
    {
        RatonesManager.OnMelodiaCorrecta += Melodia;
    }

    void OnDisable()
    {
        RatonesManager.OnMelodiaCorrecta -= Melodia;
    }

    void Melodia()
    {
        if (estado != Estado.Libre) return;
        if (Mathf.Abs(player.position.x - transform.position.x) <= radioMelodia)
        {
            tiempoQuieto = duracionQuieto;
            tiempoEsquive = 0f;
        }
    }

    void Update()
    {
        if (estado == Estado.Siguiendo)
        {
            Seguir();
            return;
        }

        if (estado == Estado.Desapareciendo) return;

        if (cooldownEsquive > 0f) cooldownEsquive -= Time.deltaTime;

        float dx = transform.position.x - player.position.x;

        // Quieto por la melodía
        if (tiempoQuieto > 0f)
        {
            tiempoQuieto -= Time.deltaTime;
            sprite.color = colorQuieto;
            PonerY(alturaSuelo);

            if (Mathf.Abs(dx) <= radioCaptura &&
                Mathf.Abs(player.position.y - transform.position.y) < 2f)
            {
                Capturar();
            }
            return;
        }

        sprite.color = colorNormal;

        // Esquive: se lanza hacia el jugador para escapar de la esquina
        if (tiempoEsquive > 0f)
        {
            tiempoEsquive -= Time.deltaTime;
            MoverX(dirEsquive, velocidadEsquive);
            Saltar(alturaSaltoHuida * 2f);
            return;
        }

        float dist = Mathf.Abs(dx);

        if (dist >= radioHuida)
        {
            PonerY(alturaSuelo);
            return;
        }

        float dir = dx >= 0f ? 1f : -1f;

        bool acorralado =
            (dir > 0f && transform.position.x >= limiteDer - radioEsquina) ||
            (dir < 0f && transform.position.x <= limiteIzq + radioEsquina);

        if (acorralado && dist < radioJugadorEsquina && cooldownEsquive <= 0f)
        {
            tiempoEsquive = duracionEsquive;
            dirEsquive = -dir;
            cooldownEsquive = duracionEsquive + esperaEntreEsquives;
            return;
        }

        // Cuanto más cerca está el jugador, más rápido huye
        float cercania = 1f - dist / radioHuida;
        float vel = Mathf.Lerp(velocidadMin, velocidadMax, cercania);

        MoverX(dir, vel);
        Saltar(alturaSaltoHuida);
    }

    void MoverX(float dir, float vel)
    {
        float x = transform.position.x + dir * vel * Time.deltaTime;
        x = Mathf.Clamp(x, limiteIzq, limiteDer);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
        sprite.flipX = dir < 0f;
    }

    void Saltar(float altura)
    {
        float y = alturaSuelo + Mathf.Abs(Mathf.Sin(Time.time * frecuenciaSaltoHuida + fase)) * altura;
        PonerY(y);
    }

    void PonerY(float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    void Capturar()
    {
        estado = Estado.Siguiendo;
        sprite.color = colorQuieto;
        indiceSeguir = manager.RatonCapturado(this);
    }

    void Seguir()
    {
        float objetivo = player.position.x - manager.DireccionJugador * separacionSeguir * (indiceSeguir + 1);
        objetivo = Mathf.Clamp(objetivo, limiteIzq, limiteDer);

        float xActual = transform.position.x;
        float t = 1f - Mathf.Exp(-velocidadSeguir * Time.deltaTime);
        float x = Mathf.Lerp(xActual, objetivo, t);

        bool moviendo = Mathf.Abs(objetivo - x) > 0.05f;
        float y = alturaSuelo;
        if (moviendo)
            y += Mathf.Abs(Mathf.Sin(Time.time * frecuenciaSalto + indiceSeguir)) * alturaSalto;

        if (Mathf.Abs(objetivo - xActual) > 0.05f)
            sprite.flipX = objetivo < xActual;

        transform.position = new Vector3(x, y, transform.position.z);
    }

    public void Desaparecer()
    {
        if (estado == Estado.Desapareciendo) return;
        estado = Estado.Desapareciendo;
        StartCoroutine(Encoger());
    }

    IEnumerator Encoger()
    {
        Vector3 inicial = transform.localScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.4f;
            transform.localScale = Vector3.Lerp(inicial, Vector3.zero, Mathf.Clamp01(t));
            yield return null;
        }
        gameObject.SetActive(false);
    }
}