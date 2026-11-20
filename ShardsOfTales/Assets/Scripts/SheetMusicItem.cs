using UnityEngine;

public class SheetMusicItem : MonoBehaviour, IInteractable
{
    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (collected || PiperQuestManager.Instance == null || SimpleDialogueUI.Instance == null) return;

        if (PiperQuestManager.Instance.CurrentStep == PiperQuestManager.Step.TalkToPiper)
        {
            SimpleDialogueUI.Instance.ShowDialogue("Bibliotecario", new string[]
            {
                "Una partitura antigua... Debería hablar primero con el Flautista."
            });
            return;
        }

        collected = true;
        PiperQuestManager.Instance.CurrentStep = PiperQuestManager.Step.PlayOcarina;

        SimpleDialogueUI.Instance.ShowDialogue("Partitura Recuperada", new string[]
        {
            "¡Has recogido la partitura del Flautista!",
            "Vuelve junto a él para tocar la melodía."
        });

        gameObject.SetActive(false);
    }
}
