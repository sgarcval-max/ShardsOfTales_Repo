using UnityEngine;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private string npcName = "Bibliotecario Antiguo";
    [TextArea(2, 5)]
    [SerializeField] private string dialogueMessage = "¡Cuidado! Los cuentos se están corrompiendo y sus finales están cambiando...";

    public void Interact()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue(npcName, dialogueMessage);
        }
    }
}