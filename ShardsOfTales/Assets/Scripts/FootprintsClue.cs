using UnityEngine;

public class FootprintsClue : MonoBehaviour, IInteractable
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) Interact();
    }

    public void Interact()
    {
        if (SimpleDialogueUI.Instance == null || PiperQuestManager.Instance == null) return;

        if (PiperQuestManager.Instance.CurrentStep == PiperQuestManager.Step.CheckFootprints)
        {
            PiperQuestManager.Instance.CurrentStep = PiperQuestManager.Step.FindCrippledKids;
            SimpleDialogueUI.Instance.ShowDialogue("Pista: Huellas en el Barro", new string[]
            {
                "Observas unas huellas en el suelo: una pisada normal y otra arrastrada...",
                "¡Son las huellas del niño cojo! Siguen recto hacia el pie de la montaña."
            });
        }
    }
}