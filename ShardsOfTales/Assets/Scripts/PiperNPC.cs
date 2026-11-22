using UnityEngine;

public class PiperNPC : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (SimpleDialogueUI.Instance == null || PiperQuestManager.Instance == null) return;
        if (OcarinaMinigameUI.Instance != null && OcarinaMinigameUI.Instance.IsOpen) return;

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
                    "¡Esa es mi partitura! Toma mi flauta y toca la melodía exactamente como aparece.",
                    "Con esa canción todas las ratas te seguirán en fila."
                }, () =>
                {
                    if (OcarinaMinigameUI.Instance != null)
                    {
                        OcarinaMinigameUI.Instance.StartMinigame();
                    }
                });
                break;

            case PiperQuestManager.Step.HerdRatsToForest:
                SimpleDialogueUI.Instance.ShowDialogue("Flautista de Hamelín", new string[]
                {
                    "¡La melodía sigue sonando! Acércate a las 5 ratas del pueblo para que te sigan y llévalas al bosque de la izquierda."
                });
                break;

            case PiperQuestManager.Step.ReturnToPiper:
                SimpleDialogueUI.Instance.ShowDialogue("Flautista de Hamelín", new string[]
                {
                    "¡Increíble! No queda ni una sola rata en Hamelín.",
                    "¿Qué...? ¿Que el alcalde dice que era una broma y NO ME VA A PAGAR?",
                    "¡Me han engañado! ¡Ahora tocaré otra melodía y me llevaré lo que más quieren: a todos los niños del pueblo!"
                }, () =>
                {
                    PiperQuestManager.Instance.StartPuzzle2AfterPiperLeaves(gameObject);
                });
                break;
        }
    }
}
