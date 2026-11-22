using UnityEngine;

public class BlindChildNPC : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (SimpleDialogueUI.Instance == null || PiperQuestManager.Instance == null) return;

        if (PiperQuestManager.Instance.CurrentStep == PiperQuestManager.Step.FindBlindChild)
        {
            SimpleDialogueUI.Instance.ShowDialogue("Niño Ciego", new string[]
            {
                "¡Por fin oigo pasos! Me quedé atrás porque no veo por dónde cruzar este río...",
                "Escucho unas piedras moviéndose en el agua. ¿Me guías saltando de piedra en piedra hasta la otra orilla?"
            }, () =>
            {
                if (RiverCrossyMinigame.Instance != null)
                {
                    RiverCrossyMinigame.Instance.StartMinigame();
                }
            });
        }
        else if (PiperQuestManager.Instance.CurrentStep >= PiperQuestManager.Step.CheckFootprints)
        {
            SimpleDialogueUI.Instance.ShowDialogue("Niño Ciego", new string[]
            {
                "¡Gracias por cruzarme al otro lado! Escuché al niño cojo ir hacia la montaña."
            });
        }
        else
        {
            SimpleDialogueUI.Instance.ShowDialogue("Niño Ciego", new string[]
            {
                "Estoy esperando en la orilla del río... Habla primero con el niño del lago."
            });
        }
    }
}