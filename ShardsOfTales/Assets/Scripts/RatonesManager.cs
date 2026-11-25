using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class RatonesManager : MonoBehaviour
{
    public static event Action OnMelodiaCorrecta;

    public TMP_Text textoContador;
    public TMP_Text textoMensaje;
    public int totalRatones = 5;
    public int[] melodia = { 1, 3, 5 };
    public bool forzarFlautaParaPruebas = true;

    private int capturados;
    private List<int> ultimasNotas = new List<int>();
    private Coroutine rutina;

    void Awake()
    {
        if (forzarFlautaParaPruebas) GameState.tieneFlauta = true;
    }

    void Start()
    {
        textoMensaje.gameObject.SetActive(false);

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

    public void RatonCapturado()
    {
        capturados++;
        ActualizarContador();

        if (capturados >= totalRatones)
        {
            GameState.ratonesCompletados = true;
            if (rutina != null) StopCoroutine(rutina);
            textoMensaje.text = "¡Has capturado a todos los ratones!";
            textoMensaje.gameObject.SetActive(true);
            Debug.Log("Todos los ratones capturados");
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
