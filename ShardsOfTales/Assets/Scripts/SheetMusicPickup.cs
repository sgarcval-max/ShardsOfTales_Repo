using UnityEngine;

public class SheetMusicPickup : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    [SerializeField] private NPCInteractable piperNpc;

    [Header("Animación Flotante")]
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatHeight = 0.2f;

    private Vector3 startPos;
    private bool isCollected = false;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    public void Interact()
    {
        Collect();
    }

    private void Collect()
    {
        if (isCollected) return;

        if (StoryProgressManager.Instance != null && !StoryProgressManager.Instance.HasPiperDisguise)
        {
            if (DialogueManager.Instance != null && !DialogueManager.Instance.IsDialogueActive)
            {
                DialogueManager.Instance.StartDialogue("Bibliotecario", new string[] {
                    "Una partitura antigua... Debería hablar primero con el Flautista antes de cogerla."
                });
            }
            return;
        }

        isCollected = true;

        if (StoryProgressManager.Instance != null)
        {
            StoryProgressManager.Instance.CollectSheetMusic();
        }

        // Cambia al Flautista a su diálogo secundario para iniciar el minijuego
        if (piperNpc == null) piperNpc = FindObjectOfType<NPCInteractable>();
        if (piperNpc != null)
        {
            piperNpc.SetUseSecondaryDialogue(true);
        }

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue("Partitura Hipnótica", new string[] {
                "¡Has encontrado la partitura de Hamelín!",
                "Vuelve al inicio del pueblo junto al Flautista para tocar la melodía y llevarte a las ratas."
            });
        }

        gameObject.SetActive(false);
    }
}
