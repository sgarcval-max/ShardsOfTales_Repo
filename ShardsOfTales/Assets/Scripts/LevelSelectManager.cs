using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager Instance;

    public enum HubState
    {
        Exploring,   // Caminando libremente por la biblioteca 3D
        ShelfView,   // Mirando la estantería de cerca para elegir libro
        BookView     // Mirando el libro abierto en el atril
    }

    public HubState CurrentState { get; private set; } = HubState.Exploring;

    [Header("Seguimiento del Jugador (Modo Exploración)")]
    [SerializeField] private PlayerController3D playerController;
    [SerializeField] private Vector3 cameraFollowOffset = new Vector3(0f, 5.5f, -6.5f);
    [SerializeField] private Vector3 cameraFollowRotation = new Vector3(32f, 0f, 0f);
    [SerializeField] private float followSmoothSpeed = 6f;

    [Header("Puntos de Cámara (Estantería y Atril)")]
    [SerializeField] private Transform shelfCameraPoint;
    [SerializeField] private Transform lecternCameraPoint;
    [SerializeField] private float transitionSpeed = 2.5f;

    [Header("Punto de Destino del Libro Cerrado")]
    [SerializeField] private Transform lecternBookTarget;

    [Header("Interfaz UI")]
    [SerializeField] private GameObject exitShelfButton;   // Botón para volver a caminar por el Hub
    [SerializeField] private GameObject backToShelfButton; // Botón para volver del atril a la estantería

    [Header("Elementos del Atril")]
    [SerializeField] private GameObject openBookObject;
    [SerializeField] private Animator openBookAnimator;

    // Propiedad para que los libros sepan si pueden reaccionar al ratón
    public bool CanSelectBooks => CurrentState == HubState.ShelfView;
    public bool IsInspectingBook => CurrentState == HubState.BookView;

    private SelectableBook currentSelectedBook;
    private Vector3 selectedBookOriginalPos;
    private Quaternion selectedBookOriginalRot;
    private Transform mainCamTransform;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        mainCamTransform = Camera.main.transform;

        if (openBookObject != null) openBookObject.SetActive(false);
        if (exitShelfButton != null) exitShelfButton.SetActive(false);
        if (backToShelfButton != null) backToShelfButton.SetActive(false);

        CurrentState = HubState.Exploring;
        if (playerController != null) playerController.CanMove = true;
    }

    private void LateUpdate()
    {
        // Mientras exploramos la biblioteca, la cámara sigue suavemente al jugador 3D
        if (CurrentState == HubState.Exploring && !isTransitioning && playerController != null)
        {
            Vector3 desiredPosition = playerController.transform.position + cameraFollowOffset;
            mainCamTransform.position = Vector3.Lerp(mainCamTransform.position, desiredPosition, Time.deltaTime * followSmoothSpeed);

            Quaternion desiredRotation = Quaternion.Euler(cameraFollowRotation);
            mainCamTransform.rotation = Quaternion.Slerp(mainCamTransform.rotation, desiredRotation, Time.deltaTime * followSmoothSpeed);
        }
    }

    // 1. Llamado al hacer clic en la estantería mientras caminas
    public void EnterShelfView()
    {
        if (CurrentState != HubState.Exploring || isTransitioning) return;

        if (playerController != null) playerController.CanMove = false;
        StartCoroutine(TransitionExploreToShelf());
    }

    // 2. Llamado al pulsar el botón "Salir de Estantería"
    public void ExitShelfView()
    {
        if (CurrentState != HubState.ShelfView || isTransitioning) return;

        if (exitShelfButton != null) exitShelfButton.SetActive(false);
        StartCoroutine(TransitionShelfToExplore());
    }

    // 3. Llamado al hacer clic en un libro de la estantería
    public void InspectBook(SelectableBook book)
    {
        if (CurrentState != HubState.ShelfView || isTransitioning) return;

        currentSelectedBook = book;
        CurrentState = HubState.BookView;

        selectedBookOriginalPos = book.TrueHomePosition;
        selectedBookOriginalRot = book.transform.rotation;

        if (exitShelfButton != null) exitShelfButton.SetActive(false);
        if (backToShelfButton != null) backToShelfButton.SetActive(false);

        StartCoroutine(TransitionToLecternSequence());
    }

    // 4. Llamado al pulsar el botón "<- Volver a Estantería" desde el atril
    public void ReturnToShelf()
    {
        if (CurrentState != HubState.BookView || isTransitioning) return;

        if (backToShelfButton != null) backToShelfButton.SetActive(false);
        StartCoroutine(TransitionToShelfSequence());
    }

    private IEnumerator TransitionExploreToShelf()
    {
        isTransitioning = true;
        float t = 0f;
        Vector3 startPos = mainCamTransform.position;
        Quaternion startRot = mainCamTransform.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            mainCamTransform.position = Vector3.Lerp(startPos, shelfCameraPoint.position, t);
            mainCamTransform.rotation = Quaternion.Slerp(startRot, shelfCameraPoint.rotation, t);
            yield return null;
        }

        mainCamTransform.position = shelfCameraPoint.position;
        mainCamTransform.rotation = shelfCameraPoint.rotation;

        CurrentState = HubState.ShelfView;
        isTransitioning = false;

        if (exitShelfButton != null) exitShelfButton.SetActive(true);
    }

    private IEnumerator TransitionShelfToExplore()
    {
        isTransitioning = true;
        float t = 0f;
        Vector3 startPos = mainCamTransform.position;
        Quaternion startRot = mainCamTransform.rotation;

        Vector3 targetPos = playerController != null
            ? playerController.transform.position + cameraFollowOffset
            : startPos;
        Quaternion targetRot = Quaternion.Euler(cameraFollowRotation);

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            mainCamTransform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamTransform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        CurrentState = HubState.Exploring;
        isTransitioning = false;

        if (playerController != null) playerController.CanMove = true;
    }

    private IEnumerator TransitionToLecternSequence()
    {
        isTransitioning = true;
        float t = 0f;
        Vector3 camStartPos = mainCamTransform.position;
        Quaternion camStartRot = mainCamTransform.rotation;

        Vector3 bookStartPos = currentSelectedBook.transform.position;
        Quaternion bookStartRot = currentSelectedBook.transform.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;

            mainCamTransform.position = Vector3.Lerp(camStartPos, lecternCameraPoint.position, t);
            mainCamTransform.rotation = Quaternion.Slerp(camStartRot, lecternCameraPoint.rotation, t);

            if (currentSelectedBook != null && lecternBookTarget != null)
            {
                currentSelectedBook.transform.position = Vector3.Lerp(bookStartPos, lecternBookTarget.position, t);
                currentSelectedBook.transform.rotation = Quaternion.Slerp(bookStartRot, lecternBookTarget.rotation, t);
            }

            yield return null;
        }

        mainCamTransform.position = lecternCameraPoint.position;
        mainCamTransform.rotation = lecternCameraPoint.rotation;

        if (currentSelectedBook != null) currentSelectedBook.gameObject.SetActive(false);

        if (openBookObject != null)
        {
            openBookObject.SetActive(true);
            if (openBookAnimator != null)
            {
                openBookAnimator.SetTrigger("OpenBook");
            }
        }

        isTransitioning = false;
        if (backToShelfButton != null) backToShelfButton.SetActive(true);
    }

    private IEnumerator TransitionToShelfSequence()
    {
        isTransitioning = true;

        if (openBookAnimator != null)
        {
            openBookAnimator.SetTrigger("CloseBook");
        }

        yield return new WaitForSeconds(0.3f);

        if (openBookObject != null) openBookObject.SetActive(false);

        if (currentSelectedBook != null)
        {
            currentSelectedBook.gameObject.SetActive(true);
            currentSelectedBook.transform.position = lecternBookTarget.position;
            currentSelectedBook.transform.rotation = lecternBookTarget.rotation;
        }

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

        if (currentSelectedBook != null)
        {
            currentSelectedBook.transform.position = selectedBookOriginalPos;
            currentSelectedBook.transform.rotation = selectedBookOriginalRot;
            currentSelectedBook.ResetBookState();
        }

        CurrentState = HubState.ShelfView;
        isTransitioning = false;

        if (exitShelfButton != null) exitShelfButton.SetActive(true);
    }

    public void EnterCurrentLevel()
    {
        if (currentSelectedBook != null)
        {
            SceneManager.LoadScene(currentSelectedBook.sceneToLoad);
        }
    }
}