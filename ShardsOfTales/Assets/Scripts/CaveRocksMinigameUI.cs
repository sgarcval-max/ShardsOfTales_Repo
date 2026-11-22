using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaveRocksMinigameUI : MonoBehaviour
{
    public static CaveRocksMinigameUI Instance { get; private set; }

    [Header("Rocas que taponan la cueva (5 rocas)")]
    public GameObject[] caveRocks;

    [Header("UI del Minijuego")]
    public GameObject rocksPanel;
    public Slider rockProgressBar;
    public TMP_Text rocksStatusText;

    public bool IsOpen { get; private set; }

    private int clearedRocks = 0;
    private float currentRockProgress = 0f;

    private void Awake()
    {
        Instance = this;
        if (rocksPanel != null) rocksPanel.SetActive(false);
    }

    private void Update()
    {
        if (!IsOpen) return;

        // La roca pesa y va bajando un poco si dejas de pulsar
        currentRockProgress = Mathf.Max(0f, currentRockProgress - Time.deltaTime * 0.25f);

        // Pulsa E o Espacio repetidamente para empujar y apartar cada roca
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            currentRockProgress += 0.24f;
            if (currentRockProgress >= 1f)
            {
                currentRockProgress = 0f;
                if (caveRocks != null && clearedRocks < caveRocks.Length && caveRocks[clearedRocks] != null)
                {
                    caveRocks[clearedRocks].SetActive(false);
                }
                clearedRocks++;

                int total = (caveRocks != null && caveRocks.Length > 0) ? caveRocks.Length : 5;
                if (clearedRocks >= total)
                {
                    CompleteMinigame();
                    return;
                }
            }
        }

        if (rockProgressBar != null) rockProgressBar.value = currentRockProgress;
        if (rocksStatusText != null)
        {
            int total = (caveRocks != null && caveRocks.Length > 0) ? caveRocks.Length : 5;
            rocksStatusText.text = $"¡Pulsa E o ESPACIO rápido para quitar las rocas! ({clearedRocks}/{total})";
        }
    }

    public void StartMinigame()
    {
        IsOpen = true;
        clearedRocks = 0;
        currentRockProgress = 0f;

        if (PiperQuestManager.Instance != null)
            PiperQuestManager.Instance.CurrentStep = PiperQuestManager.Step.ClearCaveRocks;

        if (rocksPanel != null) rocksPanel.SetActive(true);
        if (rockProgressBar != null) rockProgressBar.value = 0f;
    }

    private void CompleteMinigame()
    {
        IsOpen = false;
        if (rocksPanel != null) rocksPanel.SetActive(false);

        if (PiperQuestManager.Instance != null)
            PiperQuestManager.Instance.CurrentStep = PiperQuestManager.Step.EnterCave;

        if (SimpleDialogueUI.Instance != null)
        {
            SimpleDialogueUI.Instance.ShowDialogue("¡Cueva Despejada!", new string[]
            {
                "¡Has apartado todas las rocas de la entrada!",
                "Ahora entra en la cueva para enfrentarte al Flautista de Hamelín."
            });
        }
    }
}
