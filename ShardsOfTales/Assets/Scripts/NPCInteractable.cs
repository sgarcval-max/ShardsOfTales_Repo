using UnityEngine;
using UnityEngine.Events;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [Header("Datos del Personaje")]
    [SerializeField] private string npcName = "Flautista";
    [SerializeField] private float maxInteractDistance = 5f;

    [Header("Diálogo Inicial")]
    [TextArea(2, 4)]
    [SerializeField] private string[] initialDialogue;
    public UnityEvent onDialogueFinished;

    [Header("Diálogo Secundario (Ej. Al volver con un objeto)")]
    [TextArea(2, 4)]
    [SerializeField] private string[] secondaryDialogue;
    public UnityEvent onSecondaryDialogueFinished;
    private bool useSecondaryDialogue = false;

    [Header("Animación al Hablar (Ej. Gesto 6-7)")]
    [SerializeField] private Animator npcAnimator;
    [SerializeField] private string talkAnimTrigger;

    private Transform playerTransform;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    public void Interact()
    {
        if (DialogueManager.Instance == null || DialogueManager.Instance.IsDialogueActive) return;

        if (playerTransform != null)
        {
            float dist = Vector3.Distance(playerTransform.position, transform.position);
            if (dist > maxInteractDistance)
            {
                Debug.Log("Acércate más para hablar con " + npcName);
                return;
            }
        }

        if (npcAnimator != null && !string.IsNullOrEmpty(talkAnimTrigger))
        {
            npcAnimator.SetTrigger(talkAnimTrigger);
        }

        bool playingSecondary = useSecondaryDialogue && secondaryDialogue != null && secondaryDialogue.Length > 0;
        string[] linesToShow = playingSecondary ? secondaryDialogue : initialDialogue;

        DialogueManager.Instance.StartDialogue(npcName, linesToShow, () =>
        {
            if (playingSecondary)
            {
                onSecondaryDialogueFinished?.Invoke();
            }
            else
            {
                onDialogueFinished?.Invoke();
            }
        });
    }

    public void SetUseSecondaryDialogue(bool value)
    {
        useSecondaryDialogue = value;
    }
}
