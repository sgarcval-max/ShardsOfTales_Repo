using TMPro;
using UnityEngine;

public class RatMinigameController : MonoBehaviour
{
    public static RatMinigameController Instance;

    [Header("Interfaz del Minijuego (Partitura)")]
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private RectTransform movingNote;
    [SerializeField] private RectTransform hitZone;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Ajustes de Dificultad")]
    [SerializeField] private int notesRequiredToWin = 5;
    [SerializeField] private float noteSpeed = 420f;
    [SerializeField] private float hitTolerance = 55f;
    [SerializeField] private float trackStartX = 280f;
    [SerializeField] private float trackEndX = -280f;

    [Header("Objetos de Escena (Acto 1 -> Acto 2)")]
    [SerializeField] private GameObject ratsGroupObject; // Padre de las ratas en la escena
    [SerializeField] private GameObject act2WorldGroup;  // Padre de los niños del Acto 2

    private PlayerController25D playerController;
    private bool isPlaying = false;
    private int successfulNotes = 0;
    private float currentSpeed;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController25D>();

        if (minigamePanel != null) minigamePanel.SetActive(false);
        if (ratsGroupObject != null) ratsGroupObject.SetActive(true);
        if (act2WorldGroup != null) act2WorldGroup.SetActive(false);
    }

    private void Update()
    {
        if (!isPlaying || movingNote == null || hitZone == null) return;

        // Mover la nota musical de derecha a izquierda por el pentagrama
        Vector2 pos = movingNote.anchoredPosition;
        pos.x -= currentSpeed * Time.deltaTime;
        movingNote.anchoredPosition = pos;

        // Si la nota se pasa sin pulsarla, vuelve a empezar
        if (pos.x < trackEndX)
        {
            if (feedbackText != null) feedbackText.text = "¡Se escapó la nota! Pulsa ESPACIO en la marca.";
            ResetNotePosition();
            return;
        }

        // Comprobar pulsación del jugador (Espacio o Clic Izquierdo)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            float distance = Mathf.Abs(movingNote.anchoredPosition.x - hitZone.anchoredPosition.x);

            if (distance <= hitTolerance)
            {
                successfulNotes++;
                if (feedbackText != null) feedbackText.text = "¡Nota perfecta! Las ratas te siguen...";
                UpdateProgressUI();

                if (successfulNotes >= notesRequiredToWin)
                {
                    WinMinigame();
                }
                else
                {
                    // Aumentamos ligeramente la velocidad en cada nota acertada
                    currentSpeed += 35f;
                    ResetNotePosition();
                }
            }
            else
            {
                if (feedbackText != null) feedbackText.text = "¡Fallaste el ritmo! Inténtalo con la siguiente nota.";
                ResetNotePosition();
            }
        }
    }

    public void StartMinigame()
    {
        if (isPlaying) return;

        isPlaying = true;
        successfulNotes = 0;
        currentSpeed = noteSpeed;

        if (playerController != null) playerController.CanMove = false;
        if (minigamePanel != null) minigamePanel.SetActive(true);
        if (feedbackText != null) feedbackText.text = "Pulsa ESPACIO cuando la nota pase por el círculo.";

        UpdateProgressUI();
        ResetNotePosition();
    }

    private void ResetNotePosition()
    {
        if (movingNote != null)
        {
            movingNote.anchoredPosition = new Vector2(trackStartX, 0f);
        }
    }

    private void UpdateProgressUI()
    {
        if (progressText != null)
        {
            progressText.text = $"Ratas hipnotizadas: {successfulNotes} / {notesRequiredToWin}";
        }
    }

    private void WinMinigame()
    {
        isPlaying = false;
        if (minigamePanel != null) minigamePanel.SetActive(false);

        // Las ratas abandonan el pueblo
        if (ratsGroupObject != null) ratsGroupObject.SetActive(false);

        // Activamos los personajes y pistas del Acto 2
        if (act2WorldGroup != null) act2WorldGroup.SetActive(true);

        if (StoryProgressManager.Instance != null)
        {
            StoryProgressManager.Instance.CompleteRatMinigame();
        }

        if (playerController != null) playerController.CanMove = true;

        // Diálogo narrativo que abre el Acto 2
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue("Narrador", new string[] {
                "¡Has guiado a todas las ratas fuera de Hamelín con la melodía!",
                "Sin embargo, el verdadero Flautista se ha enfurecido al ver que nadie le pagó...",
                "¡Por venganza, ha tocado otra canción y se ha llevado a los niños del pueblo!",
                "Habla con los niños que quedaron atrás para seguir sus pistas y rescatar a los tres perdidos."
            });
        }
    }
}
