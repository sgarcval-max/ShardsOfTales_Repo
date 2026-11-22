using UnityEngine;

public class PencilItem : MonoBehaviour, IInteractable
{
    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) Interact();
    }

    public void Interact()
    {
        if (collected || PiperQuestManager.Instance == null || SimpleDialogueUI.Instance == null) return;

        collected = true;
        PiperQuestManager.Instance.CurrentStep = PiperQuestManager.Step.TalkDeafWithPencil;

        SimpleDialogueUI.Instance.ShowDialogue("Bolígrafo Encontrado", new string[]
        {
            "¡Has encontrado un bolígrafo!",
            "Vuelve junto al niño del lago para escribirle y preguntarle adónde fueron los demás."
        });

        gameObject.SetActive(false);
    }
}
