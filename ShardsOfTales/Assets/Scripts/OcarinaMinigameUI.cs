using System.Text;
using TMPro;
using UnityEngine;

public class OcarinaMinigameUI : MonoBehaviour
{
    public static OcarinaMinigameUI Instance { get; private set; }

    [Header("Referencias UI")]
    public GameObject ocarinaPanel;
    public TMP_Text notesDisplayText;
    public TMP_Text hintText;

    public bool IsOpen { get; private set; }

    private enum NoteDir { Left, Up, Right, Down }

    // Secuencia clásica estilo Zelda: Izquierda, Arriba, Derecha, Izquierda, Arriba, Derecha
    private readonly NoteDir[] melody = new NoteDir[]
    {
        NoteDir.Left, NoteDir.Up, NoteDir.Right,
        NoteDir.Left, NoteDir.Up, NoteDir.Right
    };

    private int currentNoteIndex = 0;

    private void Awake()
    {
        Instance = this;
        if (ocarinaPanel != null) ocarinaPanel.SetActive(false);
    }

    private void Update()
    {
        if (!IsOpen) return;

        NoteDir? pressedNote = null;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            pressedNote = NoteDir.Left;
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            pressedNote = NoteDir.Up;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            pressedNote = NoteDir.Right;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            pressedNote = NoteDir.Down;

        if (pressedNote == null) return;

        if (pressedNote.Value == melody[currentNoteIndex])
        {
            currentNoteIndex++;
            if (hintText != null) hintText.text = "¡Bien! Sigue la melodía...";
            UpdateNotesDisplay();

            if (currentNoteIndex >= melody.Length)
            {
                CompleteMinigame();
            }
        }
        else
        {
            currentNoteIndex = 0;
            if (hintText != null) hintText.text = "¡Nota equivocada! Empieza de nuevo la secuencia.";
            UpdateNotesDisplay();
        }
    }

    public void StartMinigame()
    {
        IsOpen = true;
        currentNoteIndex = 0;
        if (ocarinaPanel != null) ocarinaPanel.SetActive(true);
        if (hintText != null) hintText.text = "Usa WASD o las Flechas para tocar las notas:";
        UpdateNotesDisplay();
    }

    private void UpdateNotesDisplay()
    {
        if (notesDisplayText == null) return;

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < melody.Length; i++)
        {
            string label = GetNoteLabel(melody[i]);
            if (i < currentNoteIndex)
                sb.Append($"<color=#4CFF4C>{label}</color>   "); // Verde si ya la acertó
            else if (i == currentNoteIndex)
                sb.Append($"<color=#FFE600>[{label}]</color>   "); // Amarillo la siguiente a pulsar
            else
                sb.Append($"<color=#FFFFFF>{label}</color>   "); // Blanco las pendientes
        }

        notesDisplayText.text = sb.ToString();
    }

    private string GetNoteLabel(NoteDir dir)
    {
        switch (dir)
        {
            case NoteDir.Left: return "◄ A";
            case NoteDir.Up: return "▲ W";
            case NoteDir.Right: return "► D";
            default: return "▼ S";
        }
    }

    private void CompleteMinigame()
    {
        IsOpen = false;
        if (ocarinaPanel != null) ocarinaPanel.SetActive(false);

        if (PiperQuestManager.Instance != null)
        {
            PiperQuestManager.Instance.CurrentStep = PiperQuestManager.Step.HerdRatsToForest;
        }

        if (SimpleDialogueUI.Instance != null)
        {
            SimpleDialogueUI.Instance.ShowDialogue("Melodía Hipnótica", new string[]
            {
                "¡Has tocado la melodía perfectamente!",
                "Ahora recorre el pueblo (incluidas las plataformas) para atraer a las 5 ratas.",
                "Cuando te sigan en fila, llévalas hacia el bosque de la izquierda."
            });
        }
    }
}