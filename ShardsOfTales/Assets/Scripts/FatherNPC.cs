using UnityEngine;

public class FatherNPC : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (SimpleDialogueUI.Instance == null || PiperQuestManager.Instance == null) return;

        if (PiperQuestManager.Instance.CurrentStep == PiperQuestManager.Step.TalkToFather)
        {
            SimpleDialogueUI.Instance.ShowDialogue("Padre del Pueblo", new string[]
            {
                "¡Socorro, viajero! ¡El Flautista se ha llevado a todos nuestros hijos tocando una melodía extraña!",
                "Han salido hacia el este, pasando el lago y el río hacia la montaña.",
                "Te abro la verja de la derecha. ¡Por favor, sigue su rastro y tráelos de vuelta!"
            }, () =>
            {
                PiperQuestManager.Instance.CurrentStep = PiperQuestManager.Step.FindDeafChild;
                PiperQuestManager.Instance.OpenTownGate();
            });
        }
        else
        {
            SimpleDialogueUI.Instance.ShowDialogue("Padre del Pueblo", new string[]
            {
                "¡Date prisa! Sigue el camino hacia la derecha, pasado el lago."
            });
        }
    }
}
