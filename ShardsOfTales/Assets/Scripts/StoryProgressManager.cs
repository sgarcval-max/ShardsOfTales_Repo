using TMPro;
using UnityEngine;

public class StoryProgressManager : MonoBehaviour
{
    public static StoryProgressManager Instance;

    [Header("Texto de Objetivo en Pantalla (Opcional)")]
    [SerializeField] private TMP_Text objectiveText;

    public bool HasPiperDisguise { get; private set; } = false;
    public bool HasSheetMusic { get; private set; } = false;
    public bool RatsCleared { get; private set; } = false;
    public int RescuedChildrenCount { get; private set; } = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateObjective("Objetivo: Habla con el Flautista enfermo.");
    }

    public void EquipPiperDisguise()
    {
        if (HasPiperDisguise) return;
        HasPiperDisguise = true;
        UpdateObjective("Objetivo: Cruza el puente y busca la partitura.");
    }

    public void CollectSheetMusic()
    {
        if (HasSheetMusic) return;
        HasSheetMusic = true;
        UpdateObjective("Objetivo: Vuelve junto al Flautista y toca la melodía.");
    }

    public void CompleteRatMinigame()
    {
        RatsCleared = true;
        UpdateObjective("Objetivo: Busca pistas y rescata a los 3 niños perdidos (0/3).");
    }

    public void RegisterRescuedChild()
    {
        RescuedChildrenCount++;
        UpdateObjective($"Objetivo: Rescata a los niños perdidos ({RescuedChildrenCount}/3).");
    }

    private void UpdateObjective(string newText)
    {
        if (objectiveText != null)
        {
            objectiveText.text = newText;
        }
    }
}