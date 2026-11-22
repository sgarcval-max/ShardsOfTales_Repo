using UnityEngine;

public class DeafChildNPC : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (SimpleDialogueUI.Instance == null || PiperQuestManager.Instance == null) return;

        switch (PiperQuestManager.Instance.CurrentStep)
        {
            case PiperQuestManager.Step.FindDeafChild:
            case PiperQuestManager.Step.FindPencil:
                SimpleDialogueUI.Instance.ShowDialogue("Niño Sordo", new string[]
                {
                    "... (El niño te mira asustado y se señala los oídos).",
                    "No puede oír lo que le dices. Busca un lápiz o bolígrafo por esta zona para escribirle en tu libreta."
                }, () =>
                {
                    PiperQuestManager.Instance.CurrentStep = PiperQuestManager.Step.FindPencil;
                });
                break;

            case PiperQuestManager.Step.TalkDeafWithPencil:
                SimpleDialogueUI.Instance.ShowDialogue("Niño Sordo", new string[]
                {
                    "*Escribes en tu libreta: '¿Adónde se llevó el Flautista a los demás niños?'*",
                    "El niño lee la nota, asiente con fuerza y señala hacia el río del bosque.",
                    "¡Te ha indicado el camino! Sigue hacia la derecha hasta la orilla del río."
                }, () =>
                {
                    PiperQuestManager.Instance.CurrentStep = PiperQuestManager.Step.FindBlindChild;
                });
                break;

            default:
                SimpleDialogueUI.Instance.ShowDialogue("Niño Sordo", new string[]
                {
                    "*El niño sonríe y vuelve a señalar hacia el río de la derecha.*"
                });
                break;
        }
    }
}
