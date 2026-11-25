using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class FlautaPlayer : MonoBehaviour
{
    public static event Action<int> OnNotaTocada;

    public TMP_Text textoNota;

    private readonly string[] nombres = { "Do", "Re", "Mi", "Fa", "Sol" };
    private readonly KeyCode[] teclas =
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5
    };

    private PlayerPlatformer2D movimiento;
    private Coroutine rutina;

    void Start()
    {
        movimiento = GetComponent<PlayerPlatformer2D>();
        textoNota.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!GameState.tieneFlauta || !movimiento.enabled) return;

        for (int i = 0; i < teclas.Length; i++)
        {
            if (Input.GetKeyDown(teclas[i]))
                Tocar(i + 1);
        }
    }

    void Tocar(int nota)
    {
        if (rutina != null) StopCoroutine(rutina);
        rutina = StartCoroutine(MostrarNota(nombres[nota - 1]));

        if (OnNotaTocada != null) OnNotaTocada(nota);
    }

    IEnumerator MostrarNota(string nombre)
    {
        textoNota.text = "Nota: " + nombre;
        textoNota.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        textoNota.gameObject.SetActive(false);
    }
}