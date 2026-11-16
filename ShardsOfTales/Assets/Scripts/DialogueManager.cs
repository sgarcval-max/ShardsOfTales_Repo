using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Elementos UI del Diálogo")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueBodyText;

    private Queue<string> sentences = new Queue<string>();
    private Action onDialogueComplete;
    private PlayerController25D playerController;

    public bool IsDialogueActive { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        playerController = FindObjectOfType<PlayerController25D>();
    }

    private void Update()
    {
        if (!IsDialogueActive) return;

        // Permite pasar de frase pulsando Espacio o Clic Izquierdo
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(string speakerName, string[] lines, Action onComplete = null)
    {
        if (IsDialogueActive || lines == null || lines.Length == 0) return;

        IsDialogueActive = true;
        onDialogueComplete = onComplete;

        if (playerController != null) playerController.CanMove = false;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (speakerNameText != null) speakerNameText.text = speakerName;

        sentences.Clear();
        foreach (string line in lines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string currentLine = sentences.Dequeue();
        if (dialogueBodyText != null)
        {
            dialogueBodyText.text = currentLine;
        }
    }

    private void EndDialogue()
    {
        IsDialogueActive = false;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        if (playerController != null) playerController.CanMove = true;

        Action callback = onDialogueComplete;
        onDialogueComplete = null;
        callback?.Invoke();
    }
}