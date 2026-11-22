using TMPro;
using UnityEngine;

public class RiverCrossyMinigame : MonoBehaviour
{
    public static RiverCrossyMinigame Instance { get; private set; }

    [Header("Referencias de Escena")]
    public Transform playerTransform;
    public Transform blindChildTransform;
    public SimpleCameraFollow cameraFollow;
    public Transform topDownCamPoint;

    [Header("Piedras del Río (4 carriles)")]
    public Transform[] riverStones;
    public float[] stoneSpeeds = new float[] { 2.4f, 3.1f, 2.7f, 3.5f };

    [Header("UI del Minijuego")]
    public GameObject crossyHintPanel;
    public TMP_Text crossyHintText;

    public bool IsPlaying { get; private set; }

    private Rigidbody playerRb;
    private readonly float[] laneX = new float[] { 41f, 43f, 45f, 47f };
    private readonly float startBankX = 39f;
    private readonly float endBankX = 49f;

    private void Awake()
    {
        Instance = this;
        if (crossyHintPanel != null) crossyHintPanel.SetActive(false);
    }

    private void Start()
    {
        if (playerTransform != null) playerRb = playerTransform.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Mover siempre las piedras de arriba a abajo en Z para que el río tenga vida
        if (riverStones != null)
        {
            for (int i = 0; i < riverStones.Length; i++)
            {
                if (riverStones[i] == null) continue;
                float speed = (stoneSpeeds != null && i < stoneSpeeds.Length) ? stoneSpeeds[i] : 2.5f;
                float newZ = Mathf.PingPong(Time.time * speed + i * 1.7f, 6f) - 3f;
                Vector3 p = riverStones[i].position;
                riverStones[i].position = new Vector3(p.x, p.y, newZ);
            }
        }

        if (!IsPlaying || playerTransform == null) return;

        // Cámara cenital suave mirando el río desde arriba
        if (Camera.main != null && topDownCamPoint != null)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, topDownCamPoint.position, Time.deltaTime * 6f);
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, topDownCamPoint.rotation, Time.deltaTime * 6f);
        }

        // Saltos por casillas (estilo Crossy Road) con WASD o Flechas
        Vector3 pos = playerTransform.position;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            pos.x += 2f;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            pos.x = Mathf.Max(startBankX, pos.x - 2f);
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            pos.z = Mathf.Min(3f, pos.z + 1.5f);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            pos.z = Mathf.Max(-3f, pos.z - 1.5f);

        playerTransform.position = pos;

        // Comprobar si llegó a la orilla derecha
        if (pos.x >= endBankX - 0.2f)
        {
            WinRiverCrossing();
            return;
        }

        // Comprobar si está en uno de los 4 carriles de agua
        for (int i = 0; i < laneX.Length; i++)
        {
            if (Mathf.Abs(pos.x - laneX[i]) < 0.6f)
            {
                if (riverStones != null && i < riverStones.Length && riverStones[i] != null)
                {
                    float zDiff = Mathf.Abs(pos.z - riverStones[i].position.z);
                    if (zDiff <= 1.65f)
                    {
                        // Va subido encima de la piedra
                        playerTransform.position = new Vector3(pos.x, 1.3f, riverStones[i].position.z);
                    }
                    else
                    {
                        // Cayó al agua: vuelve a la orilla izquierda
                        if (crossyHintText != null) crossyHintText.text = "¡Al agua! Espera a que pase la piedra frente a ti y pulsa D.";
                        playerTransform.position = new Vector3(startBankX, 1.5f, 0f);
                    }
                }
                break;
            }
        }
    }

    public void StartMinigame()
    {
        IsPlaying = true;
        if (PiperQuestManager.Instance != null)
            PiperQuestManager.Instance.CurrentStep = PiperQuestManager.Step.RiverMinigame;

        if (cameraFollow != null) cameraFollow.isOverrideActive = true;
        if (crossyHintPanel != null) crossyHintPanel.SetActive(true);
        if (crossyHintText != null) crossyHintText.text = "Usa WASD / Flechas para cruzar saltando sobre las piedras hasta la orilla derecha.";

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.isKinematic = true;
        }
        if (playerTransform != null)
        {
            playerTransform.position = new Vector3(startBankX, 1.5f, 0f);
        }
    }

    private void WinRiverCrossing()
    {
        IsPlaying = false;
        if (crossyHintPanel != null) crossyHintPanel.SetActive(false);
        if (cameraFollow != null) cameraFollow.isOverrideActive = false;

        if (playerRb != null) playerRb.isKinematic = false;
        if (playerTransform != null) playerTransform.position = new Vector3(50f, 1.5f, 0f);
        if (blindChildTransform != null) blindChildTransform.position = new Vector3(49.2f, 1.4f, 1.2f);

        if (PiperQuestManager.Instance != null)
            PiperQuestManager.Instance.CurrentStep = PiperQuestManager.Step.CheckFootprints;

        if (SimpleDialogueUI.Instance != null)
        {
            SimpleDialogueUI.Instance.ShowDialogue("¡Río Cruzado!", new string[]
            {
                "¡Has cruzado el río con el niño ciego sano y salvo!",
                "Mira en el suelo un poco más adelante: hay unas huellas que van hacia la montaña."
            });
        }
    }
}