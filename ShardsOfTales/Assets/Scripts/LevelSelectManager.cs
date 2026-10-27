using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager Instance;

    [Header("Puntos de Cámara")]
    [SerializeField] private Transform shelfCameraPoint;
    [SerializeField] private Transform lecternCameraPoint;
    [SerializeField] private float transitionSpeed = 2.5f;

    [Header("Punto de Destino del Libro Cerrado")]
    [SerializeField] private Transform lecternBookTarget; // Posición justo sobre la mesa antes de abrirse

    [Header("Interfaz UI")]
    [SerializeField] private GameObject backToShelfButton;

    [Header("Elementos del Atril")]
    [SerializeField] private GameObject openBookObject;
    [SerializeField] private Animator openBookAnimator;

    public bool IsInspectingBook { get; private set; } = false;

    private SelectableBook currentSelectedBook;
    private Vector3 selectedBookOriginalPos;
    private Quaternion selectedBookOriginalRot;
    private Transform mainCamTransform;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        mainCamTransform = Camera.main.transform;

        if (openBookObject != null) openBookObject.SetActive(false);
        if (backToShelfButton != null) backToShelfButton.SetActive(false); // Botón oculto al inicio
    }

    public void InspectBook(SelectableBook book)
    {
        currentSelectedBook = book;
        IsInspectingBook = true;

        // Guardamos la posición limpia del hueco (TrueHomePosition), no la posición desplazada por el ratón
        selectedBookOriginalPos = book.TrueHomePosition;
        selectedBookOriginalRot = book.transform.rotation;

        if (backToShelfButton != null) backToShelfButton.SetActive(false);

        StartCoroutine(TransitionToLecternSequence());
    }

    public void ReturnToShelf()
    {
        if (backToShelfButton != null) backToShelfButton.SetActive(false);
        StartCoroutine(TransitionToShelfSequence());
    }

    private IEnumerator TransitionToLecternSequence()
    {
        float t = 0f;
        Vector3 camStartPos = mainCamTransform.position;
        Quaternion camStartRot = mainCamTransform.rotation;

        Vector3 bookStartPos = currentSelectedBook.transform.position;
        Quaternion bookStartRot = currentSelectedBook.transform.rotation;

        // 1. Desplazamiento simultáneo de la cámara y del libro volando hacia la mesa
        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;

            // Transición Cámara
            mainCamTransform.position = Vector3.Lerp(camStartPos, lecternCameraPoint.position, t);
            mainCamTransform.rotation = Quaternion.Slerp(camStartRot, lecternCameraPoint.rotation, t);

            // Transición Libro (Vuelo + Rotación horizontal hacia el atril)
            if (currentSelectedBook != null && lecternBookTarget != null)
            {
                currentSelectedBook.transform.position = Vector3.Lerp(bookStartPos, lecternBookTarget.position, t);
                currentSelectedBook.transform.rotation = Quaternion.Slerp(bookStartRot, lecternBookTarget.rotation, t);
            }

            yield return null;
        }

        mainCamTransform.position = lecternCameraPoint.position;
        mainCamTransform.rotation = lecternCameraPoint.rotation;

        // 2. Intercambio: Ocultar el libro cerrado y activar la versión desplegada/abierta
        if (currentSelectedBook != null) currentSelectedBook.gameObject.SetActive(false);

        if (openBookObject != null)
        {
            openBookObject.SetActive(true);
            if (openBookAnimator != null)
            {
                openBookAnimator.SetTrigger("OpenBook");
            }
        }

        // 3. Activar el botón de volver solo cuando el libro está en la mesa
        if (backToShelfButton != null) backToShelfButton.SetActive(true);
    }

    private IEnumerator TransitionToShelfSequence()
    {
        // 1. Transición de cierre en el atril
        if (openBookAnimator != null)
        {
            openBookAnimator.SetTrigger("CloseBook");
        }

        yield return new WaitForSeconds(0.3f);

        if (openBookObject != null) openBookObject.SetActive(false);

        // Reactivar modelo de libro cerrado sobre la mesa para el viaje de vuelta
        if (currentSelectedBook != null)
        {
            currentSelectedBook.gameObject.SetActive(true);
            currentSelectedBook.transform.position = lecternBookTarget.position;
            currentSelectedBook.transform.rotation = lecternBookTarget.rotation;
        }

        // 2. Transición inversa: Cámara regresa a la estantería y el libro vuelve a su hueco
        float t = 0f;
        Vector3 camStartPos = mainCamTransform.position;
        Quaternion camStartRot = mainCamTransform.rotation;

        Vector3 bookStartPos = lecternBookTarget.position;
        Quaternion bookStartRot = lecternBookTarget.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;

            mainCamTransform.position = Vector3.Lerp(camStartPos, shelfCameraPoint.position, t);
            mainCamTransform.rotation = Quaternion.Slerp(camStartRot, shelfCameraPoint.rotation, t);

            if (currentSelectedBook != null)
            {
                currentSelectedBook.transform.position = Vector3.Lerp(bookStartPos, selectedBookOriginalPos, t);
                currentSelectedBook.transform.rotation = Quaternion.Slerp(bookStartRot, selectedBookOriginalRot, t);
            }

            yield return null;
        }

        // Restaurar posición y estado original del libro
        if (currentSelectedBook != null)
        {
            currentSelectedBook.transform.position = selectedBookOriginalPos;
            currentSelectedBook.transform.rotation = selectedBookOriginalRot;
            currentSelectedBook.ResetBookState();
        }

        IsInspectingBook = false;
    }

    public void EnterCurrentLevel()
    {
        if (currentSelectedBook != null)
        {
            SceneManager.LoadScene(currentSelectedBook.sceneToLoad);
        }
    }
}