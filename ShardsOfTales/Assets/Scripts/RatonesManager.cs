using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class RatonesManager : MonoBehaviour
{
    public static event Action OnMelodiaCorrecta;

    public Transform player;
    public TMP_Text textoContador;
    public TMP_Text textoMensaje;
    public int totalRatones = 5;
    public int[] melodia = { 1, 3, 5 };
    public bool forzarFlautaParaPruebas = true;
    public float esperaDesaparecer = 2f;

    private int capturados;
    private List<int> ultimasNotas = new List<int>();
    private List<RatonHuidizo> seguidores = new List<RatonHuidizo>();
    private Coroutine rutina;
    private float dirJugador = 1f;
    private float ultimaX;

    public float DireccionJugador
    {
        get { return dirJugador; }
    }

    void Awake()
    {
        if (forzarFlautaParaPruebas) GameState.tieneFlauta = true;
    }

    void Start()
    {
        textoMensaje.gameObject.SetActive(false);
        ultimaX = player.position.x;

        if (GameState.ratonesCompletados)
            capturados = totalRatones;

        ActualizarContador();
    }

    void OnEnable()
    {
        FlautaPlayer.OnNotaTocada += NotaTocada;
    }

    void OnDisable()
    {
        FlautaPlayer.OnNotaTocada -= NotaTocada;
    }

    void Update()
    {
        float dx = player.position.x - ultimaX;
        if (Mathf.Abs(dx) > 0.001f)
            dirJugador = Mathf.Sign(dx);
        ultimaX = player.position.x;
    }

    void NotaTocada(int nota)
    {
        ultimasNotas.Add(nota);
        if (ultimasNotas.Count > melodia.Length)
            ultimasNotas.RemoveAt(0);

        if (ultimasNotas.Count < melodia.Length) return;

        for (int i = 0; i < melodia.Length; i++)
            if (ultimasNotas[i] != melodia[i]) return;

        ultimasNotas.Clear();
        if (OnMelodiaCorrecta != null) OnMelodiaCorrecta();
        MostrarTemporal("¡Melodía correcta!");
    }

    public int RatonCapturado(RatonHuidizo raton)
    {
        int indice = seguidores.Count;
        seguidores.Add(raton);
        capturados++;
        ActualizarContador();

        if (capturados >= totalRatones)
        {
            GameState.ratonesCompletados = true;
            if (rutina != null) StopCoroutine(rutina);
            textoMensaje.text = "¡Has capturado a todos los ratones!";
            textoMensaje.gameObject.SetActive(true);
            StartCoroutine(DesaparecenTodos());
        }

        return indice;
    }

    IEnumerator DesaparecenTodos()
    {
        yield return new WaitForSeconds(esperaDesaparecer);

        foreach (RatonHuidizo r in seguidores)
        {
            r.Desaparecer();
            yield return new WaitForSeconds(0.15f);
        }
    }

    void ActualizarContador()
    {
        textoContador.text = "Ratones: " + capturados + "/" + totalRatones;
    }

    void MostrarTemporal(string texto)
    {
        if (capturados >= totalRatones) return;
        if (rutina != null) StopCoroutine(rutina);
        rutina = StartCoroutine(Mostrar(texto));
    }

    IEnumerator Mostrar(string texto)
    {
        textoMensaje.text = texto;
        textoMensaje.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        textoMensaje.gameObject.SetActive(false);
    }
}