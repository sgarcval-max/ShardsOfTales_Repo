using UnityEngine;

public class PiperNPC : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (SimpleDialogueUI.Instance == null || PiperQuestManager.Instance == null) return;

        switch (PiperQuestManager.Instance.CurrentStep)
        {
            case PiperQuestManager.Step.TalkToPiper:
                SimpleDialogueUI.Instance.ShowDialogue("Flautista de Hamelín", new string[]
                {
                    "¡Cof, cof! Estoy demasiado enfermo para buscar mis cosas...",
                    "Las ratas han invadido la ciudad y necesito recuperar mi partitura.",
                    "Sube por las plataformas de la derecha, encuéntrala y tráemela."
                }, () =>
                {
                    PiperQuestManager.Instance.CurrentStep = PiperQuestManager.Step.FindSheetMusic;
                });
                break;

            case PiperQuestManager.Step.FindSheetMusic:
                SimpleDialogueUI.Instance.ShowDialogue("Flautista de Hamelín", new string[]
                {
                    "Mi partitura está arriba en las plataformas de la derecha. ¡Date prisa, cof, cof!"
                });
                break;

            case PiperQuestManager.Step.PlayOcarina:
                SimpleDialogueUI.Instance.ShowDialogue("Flautista de Hamelín", new string[]
                {
                    "¡Genial, has recuperado la partitura!",
                    "(Aquí empezará el minijuego de tocar la partitura en la Fase 2)."
                });
                break;
        }
    }
}
