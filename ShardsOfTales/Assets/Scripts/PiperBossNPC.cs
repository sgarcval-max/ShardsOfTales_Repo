using UnityEngine;

public class PiperBossNPC : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (SimpleDialogueUI.Instance == null || PiperQuestManager.Instance == null) return;
        if (FNFBossMinigameUI.Instance != null && FNFBossMinigameUI.Instance.IsPlaying) return;

        if (PiperQuestManager.Instance.CurrentStep == PiperQuestManager.Step.InsideCave)
        {
            SimpleDialogueUI.Instance.ShowDialogue("Flautista de Hamelín", new string[]
            {
                "¿Tú otra vez? ¡No debiste seguirme hasta aquí, viajero!",
                "Hamelín rompió su promesa y ahora estos niños me pertenecen.",
                "Si quieres liberarlos, tendrás que vencerme en un duelo musical. ¡A ver si sigues mi ritmo!"
            }, () =>
            {
                if (FNFBossMinigameUI.Instance != null)
                {
                    FNFBossMinigameUI.Instance.StartBattle();
                }
            });
        }
    }
}