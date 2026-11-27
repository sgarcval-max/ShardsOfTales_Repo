using UnityEngine;
using TMPro;
using System.Collections;

public class CastilloManager : MonoBehaviour
{
    public Transform player;
    public GameObject bestia;
    public TMP_Text textoMensaje;

    // X donde aparece el jugador: inicio, puerta comedor, puerta cocina, puerta rosa
    public float[] posicionesX = { -17f, -7f, 1f, 13f };
    public Vector3 offsetCamara = new Vector3(0f, 3f, -12f);

    public bool forzarMinijuegosParaPruebas = false;

    void Awake()
    {
        if (forzarMinijuegosParaPruebas)
        {
            GameState.dinDonHablado = true;
            GameState.velasCompletadas = true;
            GameState.cocinaCompletada = true;
        }
    }

    void Start()
    {
        int indice = Mathf.Clamp(GameState.spawnCastillo, 0, posicionesX.Length - 1);
        GameState.spawnCastillo = 0;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.position = new Vector3(posicionesX[indice], 1f, 0f);
        if (cc != null) cc.enabled = true;

        Camera cam = Camera.main;
        if (cam != null) cam.transform.position = player.position + offsetCamara;

        bool listos = GameState.velasCompletadas && GameState.cocinaCompletada;
        bestia.SetActive(listos);

        if (listos && !GameState.bestiaHablada)
            StartCoroutine(AvisoBestia());
    }

    IEnumerator AvisoBestia()
    {
        yield return new WaitForSeconds(1.2f);

        textoMensaje.text = "¡Comedor y cocina listos!\nLa Bestia quiere hablar contigo.";
        textoMensaje.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        textoMensaje.gameObject.SetActive(false);
    }
}