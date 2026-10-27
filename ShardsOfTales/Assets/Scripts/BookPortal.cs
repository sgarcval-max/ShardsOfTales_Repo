using UnityEngine;
using UnityEngine.SceneManagement;

public class BookPortal : MonoBehaviour, IInteractable
{
    [Header("Configuración del Libro/Nivel")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private string bookTitle = "El Cuento Corrompido";

    public void Interact()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            Debug.Log($"Entrando al libro: {bookTitle}...");
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning("¡Nombre de escena no configurado en este portal!");
        }
    }
}