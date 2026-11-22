using UnityEngine;

public class CrippledKidsNPC : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (SimpleDialogueUI.Instance == null || PiperQuestManager.Instance == null) return;

        if (PiperQuestManager.Instance.CurrentStep == PiperQuestManager.Step.FindCrippledKids)
        {
            SimpleDialogueUI.Instance.ShowDialogue("Niño Cojo y Niña", new string[]
            {
                "¡Aquí estamos! Seguimos al Flautista hasta el pie de esta montaña, pero yo no puedo subir tan rápido.",
                "El Flautista metió a todos los niños en esa cueva y derrumbó estas rocas para bloquearla.",
                "¡Ayúdanos a apartar las piedras de la entrada!"
            }, () =>
            {
                if (CaveRocksMinigameUI.Instance != null)
                {
                    CaveRocksMinigameUI.Instance.StartMinigame();
                }
            });
        }
        else if (PiperQuestManager.Instance.CurrentStep == PiperQuestManager.Step.EnterCave)
        {
            SimpleDialogueUI.Instance.ShowDialogue("Niño Cojo y Niña", new string[]
            {
                "¡La entrada está libre! Entra en la cueva y salva a nuestros amigos."
            });
        }
        else
        {
            SimpleDialogueUI.Instance.ShowDialogue("Niño Cojo y Niña", new string[]
            {
                "No podemos mover estas rocas... ¡Sigue las pistas desde el río primero!"
            });
        }
    }
}
