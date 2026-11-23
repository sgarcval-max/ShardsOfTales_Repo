using UnityEngine;

public class CaveEntranceZone : MonoBehaviour, IInteractable
{
    [Header("Interior de la Cueva")]
    public Transform caveInteriorSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (SimpleDialogueUI.Instance == null || PiperQuestManager.Instance == null) return;

        if (PiperQuestManager.Instance.CurrentStep == PiperQuestManager.Step.EnterCave)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null && caveInteriorSpawn != null)
            {
                Rigidbody rb = playerObj.GetComponent<Rigidbody>();
                if (rb != null) rb.linearVelocity = Vector3.zero;
                playerObj.transform.position = caveInteriorSpawn.position;
            }

            PiperQuestManager.Instance.CurrentStep = PiperQuestManager.Step.InsideCave;

            SimpleDialogueUI.Instance.ShowDialogue("Interior de la Cueva", new string[]
            {
                "Has entrado en lo profundo de la montaña.",
                "¡Mira al fondo! El Flautista tiene hipnotizados a los niños del pueblo. Acércate a hablar con él."
            });
        }
    }
}