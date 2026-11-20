using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleDialogueUI : MonoBehaviour
{
    public static SimpleDialogueUI Instance { get; private set; }

    [Header("Referencias UI")]
    public GameObject dialoguePanel;
    public TMP_Text speakerText;
    public TMP_Text messageText;

    public bool IsOpen { get; private set; }

    private Queue<string> linesQueue = new Queue<string>();
    private Action onFinishCallback;

    private void Awake()
    {
        Instance = this;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!IsOpen) return;

        // Pasar frase con Espacio, E o Clic Izquierdo
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            ShowNextLine();
        }
    }

    public void ShowDialogue(string speaker, string[] lines, Action onFinish = null)
    {
        if (IsOpen || lines == null || lines.Length == 0) return;

        IsOpen = true;
        onFinishCallback = onFinish;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (speakerText != null) speakerText.text = speaker;

        linesQueue.Clear();
        foreach (string line in lines)
        {
            linesQueue.Enqueue(line);
        }

        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (linesQueue.Count == 0)
        {
            IsOpen = false;
            if (dialoguePanel != null) dialoguePanel.SetActive(false);

            Action callback = onFinishCallback;
            onFinishCallback = null;
            callback?.Invoke();
            return;
        }

        if (messageText != null)
        {
            messageText.text = linesQueue.Dequeue();
        }
    }
}
