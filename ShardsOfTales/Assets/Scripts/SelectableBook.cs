using UnityEngine;

public class SelectableBook : MonoBehaviour
{
    [Header("Información del Libro")]
    public string bookTitle = "El Cuento Olvidado";
    public string sceneToLoad = "Level_01_2D";

    [Header("Efectos Visuales")]
    [SerializeField] private Vector3 hoverOffset = new Vector3(0f, 0f, -0.3f);
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private GameObject highlightGlow;

    private Vector3 trueHomePosition; // Posición base limpia e inmutable en la estantería
    private Vector3 targetPosition;
    private bool isHovered = false;

    // Propiedad pública para que LevelSelectManager sepa el sitio exacto del hueco en la estantería
    public Vector3 TrueHomePosition => trueHomePosition;

    private void Awake()
    {
        // Guardamos la posición original real antes de que cualquier hover la mueva
        trueHomePosition = transform.position;
        targetPosition = trueHomePosition;
    }

    private void Start()
    {
        if (highlightGlow != null)
            highlightGlow.SetActive(false);
    }

    private void Update()
    {
        // Solo aplica animación de hover si estamos en la vista de la estantería
        if (LevelSelectManager.Instance != null && !LevelSelectManager.Instance.IsInspectingBook)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
        }
    }

    private void OnMouseEnter()
    {
        if (LevelSelectManager.Instance != null && LevelSelectManager.Instance.IsInspectingBook) return;

        isHovered = true;
        targetPosition = trueHomePosition + hoverOffset;
        if (highlightGlow != null) highlightGlow.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (LevelSelectManager.Instance != null && LevelSelectManager.Instance.IsInspectingBook) return;

        isHovered = false;
        targetPosition = trueHomePosition;
        if (highlightGlow != null) highlightGlow.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (isHovered && LevelSelectManager.Instance != null && !LevelSelectManager.Instance.IsInspectingBook)
        {
            LevelSelectManager.Instance.InspectBook(this);
        }
    }

    public void ResetBookState()
    {
        isHovered = false;
        targetPosition = trueHomePosition;
        transform.position = trueHomePosition; // Vuelve forzosamente a su hueco exacto en la estantería
        if (highlightGlow != null) highlightGlow.SetActive(false);
    }
}