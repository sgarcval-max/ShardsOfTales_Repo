using UnityEngine;

public class CaveEntranceZone : MonoBehaviour, IInteractable
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) Interact();
    }

    public void Interact()
    {
        if (SimpleDialogueUI.Instance == null || PiperQuestManager.Instance == null) return;

        if (PiperQuestManager.Instance.CurrentStep == PiperQuestManager.Step.EnterCave)
        {
            SimpleDialogueUI.Instance.ShowDialogue("Entrada de la Cueva", new string[]
            {
                "¡Has completado el Puzzle 2 y encontrado el escondite del Flautista!",
                "(En la Fase 4 empezará aquí la batalla musical final estilo Friday Night Funkin')."
            });
        }
    }
}