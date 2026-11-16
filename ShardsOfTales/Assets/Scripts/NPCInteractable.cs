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

    [Header("Diálogo Alternativo (Opcional)")]
    [TextArea(2, 4)]
    [SerializeField] private string[] secondaryDialogue;
    private bool useSecondaryDialogue = false;

    [Header("Animación al Hablar (Ej. Gesto 6-7)")]
    [SerializeField] private Animator npcAnimator;
    [SerializeField] private string talkAnimTrigger;

    [Header("Evento al Terminar de Hablar")]
    public UnityEvent onDialogueFinished;

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

        // Dispara animación si tiene (ideal para el niño haciendo el 6-7 con las manos)
        if (npcAnimator != null && !string.IsNullOrEmpty(talkAnimTrigger))
        {
            npcAnimator.SetTrigger(talkAnimTrigger);
        }

        string[] linesToShow = (useSecondaryDialogue && secondaryDialogue.Length > 0)
            ? secondaryDialogue
            : initialDialogue;

        DialogueManager.Instance.StartDialogue(npcName, linesToShow, () =>
        {
            onDialogueFinished?.Invoke();
        });
    }

    public void SetUseSecondaryDialogue(bool value)
    {
        useSecondaryDialogue = value;
    }
}
