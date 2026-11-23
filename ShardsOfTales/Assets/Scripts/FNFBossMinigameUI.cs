using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FNFBossMinigameUI : MonoBehaviour
{
    public static FNFBossMinigameUI Instance { get; private set; }

    [Header("Panel Principal del Duelo FNF")]
    public GameObject fnfPanel;
    public RectTransform[] laneContainers; // 4 carriles: 0=Izq(A), 1=Abajo(S), 2=Arriba(W), 3=Der(D)
    public Slider battleBalanceBar;        // 0 = Gana Flautista, 1 = Gana Bibliotecario
    public TMP_Text statusText;
    public TMP_Text notesRemainingText;

    [Header("Panel de Derrota (Final Original)")]
    public GameObject originalEndingPanel;
    public Button retryBattleButton;
    public Button backToHubButton;

    [Header("Ajustes del Duelo")]
    public int totalNotesInSong = 24;
    public float noteSpeed = 290f;
    public float spawnInterval = 0.72f;
    public float receptorY = 175f;
    public float spawnY = -205f;
    public float hitTolerance = 55f;
    public string hubSceneName = "SCN_Hub_01";

    public bool IsPlaying { get; private set; }

    private class ActiveNote
    {
        public int lane;
        public RectTransform rect;
    }

    private readonly List<ActiveNote> activeNotes = new List<ActiveNote>();
    private readonly string[] laneSymbols = new string[] { "◄", "▼", "▲", "►" };
    private readonly Color[] laneColors = new Color[]
    {
        new Color(0.85f, 0.25f, 0.85f), // Morado (Izquierda)
        new Color(0.20f, 0.80f, 0.95f), // Cian (Abajo)
        new Color(0.25f, 0.90f, 0.35f), // Verde (Arriba)
        new Color(0.95f, 0.25f, 0.25f)  // Rojo (Derecha)
    };

    private float balance = 0.5f;
    private int spawnedCount = 0;
    private float spawnTimer = 0f;

    private void Awake()
    {
        Instance = this;
        if (fnfPanel != null) fnfPanel.SetActive(false);
        if (originalEndingPanel != null) originalEndingPanel.SetActive(false);
    }

    private void Start()
    {
        if (retryBattleButton != null)
        {
            retryBattleButton.onClick.AddListener(RetryBattle);
        }
        if (backToHubButton != null)
        {
            backToHubButton.onClick.AddListener(ReturnToHub);
        }
    }

    private void Update()
    {
        if (!IsPlaying) return;

        // 1. Generar notas nuevas hasta completar la canción
        if (spawnedCount < totalNotesInSong)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                int randomLane = Random.Range(0, 4);
                SpawnNote(randomLane);
                spawnedCount++;
                spawnTimer = spawnInterval;
                UpdateCounterText();
            }
        }

        // 2. Mover todas las notas hacia arriba (estilo FNF)
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            ActiveNote note = activeNotes[i];
            Vector2 pos = note.rect.anchoredPosition;
            pos.y += noteSpeed * Time.deltaTime;
            note.rect.anchoredPosition = pos;

            // Si la nota pasa de largo sin pulsarse -> fallo
            if (pos.y > receptorY + hitTolerance + 15f)
            {
                Destroy(note.rect.gameObject);
                activeNotes.RemoveAt(i);
                ChangeBalance(-0.08f, "¡Se escapó una nota!");
                if (!IsPlaying) return;
            }
        }

        // 3. Detectar pulsaciones del jugador (A/S/W/D o Flechas)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) CheckHit(0);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) CheckHit(1);
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) CheckHit(2);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) CheckHit(3);

        // 4. Comprobar victoria cuando ya salieron todas las notas y no queda ninguna en pantalla
        if (spawnedCount >= totalNotesInSong && activeNotes.Count == 0 && balance > 0f)
        {
            WinBattle();
        }
    }

    public void StartBattle()
    {
        ClearAllNotes();
        IsPlaying = true;
        balance = 0.5f;
        spawnedCount = 0;
        spawnTimer = 0.6f;

        if (PiperQuestManager.Instance != null)
            PiperQuestManager.Instance.CurrentStep = PiperQuestManager.Step.BossBattleFNF;

        if (originalEndingPanel != null) originalEndingPanel.SetActive(false);
        if (fnfPanel != null) fnfPanel.SetActive(true);
        if (battleBalanceBar != null) battleBalanceBar.value = balance;
        if (statusText != null) statusText.text = "¡Pulsa A, S, W, D o Flechas cuando las notas lleguen arriba!";
        UpdateCounterText();
    }

    private void SpawnNote(int lane)
    {
        if (laneContainers == null || lane < 0 || lane >= laneContainers.Length || laneContainers[lane] == null) return;

        GameObject noteObj = new GameObject("FNF_Note", typeof(RectTransform), typeof(Image));
        noteObj.transform.SetParent(laneContainers[lane], false);

        RectTransform rt = noteObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(72f, 72f);
        rt.anchoredPosition = new Vector2(0f, spawnY);

        Image img = noteObj.GetComponent<Image>();
        img.color = laneColors[lane];

        GameObject txtObj = new GameObject("Symbol", typeof(RectTransform), typeof(TextMeshProUGUI));
        txtObj.transform.SetParent(noteObj.transform, false);
        RectTransform txtRt = txtObj.GetComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmp = txtObj.GetComponent<TextMeshProUGUI>();
        tmp.text = laneSymbols[lane];
        tmp.fontSize = 36;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        activeNotes.Add(new ActiveNote { lane = lane, rect = rt });
    }

    private void CheckHit(int lane)
    {
        ActiveNote closest = null;
        float bestDist = float.MaxValue;

        foreach (ActiveNote note in activeNotes)
        {
            if (note.lane != lane) continue;
            float dist = Mathf.Abs(note.rect.anchoredPosition.y - receptorY);
            if (dist < bestDist)
            {
                bestDist = dist;
                closest = note;
            }
        }

        if (closest != null && bestDist <= hitTolerance)
        {
            Destroy(closest.rect.gameObject);
            activeNotes.Remove(closest);
            string rating = bestDist <= 22f ? "¡PERFECTO!" : "¡BIEN!";
            ChangeBalance(+0.055f, rating);
        }
        else
        {
            ChangeBalance(-0.04f, "¡A destiempo!");
        }
    }

    private void ChangeBalance(float delta, string msg)
    {
        balance = Mathf.Clamp01(balance + delta);
        if (battleBalanceBar != null) battleBalanceBar.value = balance;
        if (statusText != null) statusText.text = msg;

        if (balance <= 0f)
        {
            LoseBattle();
        }
    }

    private void UpdateCounterText()
    {
        if (notesRemainingText != null)
        {
            int left = Mathf.Max(0, totalNotesInSong - spawnedCount);
            notesRemainingText.text = $"Notas restantes: {left}";
        }
    }

    private void ClearAllNotes()
    {
        foreach (ActiveNote note in activeNotes)
        {
            if (note.rect != null) Destroy(note.rect.gameObject);
        }
        activeNotes.Clear();
    }

    // --- FINAL BUENO: GANAS AL FLAUTISTA ---
    private void WinBattle()
    {
        IsPlaying = false;
        ClearAllNotes();
        if (fnfPanel != null) fnfPanel.SetActive(false);

        if (PiperQuestManager.Instance != null)
        {
            PiperQuestManager.Instance.CurrentStep = PiperQuestManager.Step.LevelCompleted;
            if (PiperQuestManager.Instance.bossPiperObject != null)
                PiperQuestManager.Instance.bossPiperObject.SetActive(false);
        }

        if (SimpleDialogueUI.Instance != null)
        {
            SimpleDialogueUI.Instance.ShowDialogue("¡Victoria Musical!", new string[]
            {
                "¡Has superado la melodía del Flautista de Hamelín!",
                "Su flauta se rompe y el Flautista huye derrotado de la montaña.",
                "¡El hechizo desaparece y todos los niños vuelven a sus casas felices!",
                "Has restaurado el cuento de Hamelín. Regresando a la biblioteca..."
            }, () =>
            {
                ReturnToHub();
            });
        }
    }

    // --- FINAL ORIGINAL: PIERDES CONTRA EL FLAUTISTA ---
    private void LoseBattle()
    {
        IsPlaying = false;
        ClearAllNotes();
        if (fnfPanel != null) fnfPanel.SetActive(false);

        if (PiperQuestManager.Instance != null)
        {
            if (PiperQuestManager.Instance.bossPiperObject != null)
                PiperQuestManager.Instance.bossPiperObject.SetActive(false);
            if (PiperQuestManager.Instance.kidnappedChildrenGroup != null)
                PiperQuestManager.Instance.kidnappedChildrenGroup.SetActive(false);
        }

        if (originalEndingPanel != null)
        {
            originalEndingPanel.SetActive(true);
        }
    }

    public void RetryBattle()
    {
        if (originalEndingPanel != null) originalEndingPanel.SetActive(false);

        if (PiperQuestManager.Instance != null)
        {
            if (PiperQuestManager.Instance.bossPiperObject != null)
                PiperQuestManager.Instance.bossPiperObject.SetActive(true);
            if (PiperQuestManager.Instance.kidnappedChildrenGroup != null)
                PiperQuestManager.Instance.kidnappedChildrenGroup.SetActive(true);
        }

        StartBattle();
    }

    public void ReturnToHub()
    {
        SceneManager.LoadScene(hubSceneName);
    }
}