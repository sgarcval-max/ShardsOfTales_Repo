using UnityEngine;

public class LevelNode : MonoBehaviour
{
    [Header("Configuración de la Fase")]
    [SerializeField] private string nodeName = "Nivel 1: El Bosque Encantado";
    [SerializeField] private bool isCorrupted = true; // Si está infectado por la tinta negra

    [Header("Visuales")]
    [SerializeField] private GameObject corruptionInkVisual;

    private void Start()
    {
        if (corruptionInkVisual != null)
        {
            corruptionInkVisual.SetActive(isCorrupted);
        }
    }

    private void OnMouseDown()
    {
        Debug.Log($"Entrando a la fase 2D: {nodeName}");
        if (LevelSelectManager.Instance != null)
        {
            LevelSelectManager.Instance.EnterCurrentLevel();
        }
    }
}