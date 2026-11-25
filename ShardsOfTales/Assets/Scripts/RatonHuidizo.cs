using UnityEngine;

public class RatonHuidizo : MonoBehaviour
{
    public Transform player;
    public RatonesManager manager;
    public SpriteRenderer sprite;

    public float velocidad = 7f;
    public float radioHuida = 6f;
    public float radioCaptura = 1.3f;
    public float radioMelodia = 14f;
    public float duracionQuieto = 8f;
    public float limiteIzq = -19f;
    public float limiteDer = 19f;

    public Color colorNormal = new Color(0.55f, 0.55f, 0.55f);
    public Color colorQuieto = new Color(0.4f, 0.7f, 1f);

    private float tiempoQuieto;
    private bool capturado;

    void Start()
    {
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
        if (capturado) return;
        if (Mathf.Abs(player.position.x - transform.position.x) <= radioMelodia)
            tiempoQuieto = duracionQuieto;
    }

    void Update()
    {
        if (capturado) return;

        float dx = transform.position.x - player.position.x;

        if (tiempoQuieto > 0f)
        {
            tiempoQuieto -= Time.deltaTime;
            sprite.color = colorQuieto;

            if (Mathf.Abs(dx) <= radioCaptura &&
                Mathf.Abs(player.position.y - transform.position.y) < 2f)
            {
                capturado = true;
                manager.RatonCapturado();
                gameObject.SetActive(false);
            }
            return;
        }

        sprite.color = colorNormal;

        if (Mathf.Abs(dx) < radioHuida)
        {
            float dir = Mathf.Sign(dx);
            float x = transform.position.x + dir * velocidad * Time.deltaTime;
            x = Mathf.Clamp(x, limiteIzq, limiteDer);
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
    }
}