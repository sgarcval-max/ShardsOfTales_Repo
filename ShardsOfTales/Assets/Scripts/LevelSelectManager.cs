using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager Instance;

    public enum HubState
    {
        MainMenu,    // Menú principal en el centro mientras la cámara recorre el Hub
        Exploring,   // Caminando libremente por la biblioteca 3D
        ShelfView,   // Mirando la estantería de cerca para elegir libro
        BookView     // Mirando el libro abierto en el atril
    }

    public HubState CurrentState { get; private set; } = HubState.MainMenu;

    [Header("Fundido de Entrada y Menú Principal")]
    [SerializeField] private Image blackFadeOverlay;
    [SerializeField] private float entryFadeDuration = 1.2f;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Cámara Cinemática del Menú")]
    [SerializeField] private Transform menuCamPointA;
    [SerializeField] private Transform menuCamPointB;
    [SerializeField] private float menuPanSpeed = 0.08f;

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

    [Header("Interfaz UI del Hub")]
    [SerializeField] private GameObject returnToMenuButton; // Botón Volver (abajo a la derecha) al explorar
    [SerializeField] private GameObject exitShelfButton;    // Botón para volver a caminar por el Hub
    [SerializeField] private GameObject backToShelfButton;  // Botón para volver del atril a la estantería

    [Header("Elementos del Atril")]
    [SerializeField] private GameObject openBookObject;
    [SerializeField] private Animator openBookAnimator;

    public bool CanSelectBooks => CurrentState == HubState.ShelfView;
    public bool IsInspectingBook => CurrentState == HubState.BookView;

    private SelectableBook currentSelectedBook;
    private Vector3 selectedBookOriginalPos;
    private Quaternion selectedBookOriginalRot;
    private Transform mainCamTransform;
    private bool isTransitioning = false;
    private float menuPanTimer = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        mainCamTransform = Camera.main.transform;

        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController3D>();
        }

        // Empezamos en modo Menú Principal
        CurrentState = HubState.MainMenu;
        if (playerController != null) playerController.CanMove = false;

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (openBookObject != null) openBookObject.SetActive(false);
        if (returnToMenuButton != null) returnToMenuButton.SetActive(false);
        if (exitShelfButton != null) exitShelfButton.SetActive(false);
        if (backToShelfButton != null) backToShelfButton.SetActive(false);

        if (menuCamPointA != null)
        {
            mainCamTransform.position = menuCamPointA.position;
            mainCamTransform.rotation = menuCamPointA.rotation;
        }

        // Hacemos Fade Out del fondo negro al entrar desde SCN_Intro
        if (blackFadeOverlay != null)
        {
            blackFadeOverlay.gameObject.SetActive(true);
            Color c = blackFadeOverlay.color;
            c.a = 1f;
            blackFadeOverlay.color = c;
            StartCoroutine(FadeOutEntryBlackScreen());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentState == HubState.ShelfView)
            {
                ExitShelfView();
            }
            else if (CurrentState == HubState.BookView)
            {
                ReturnToShelf();
            }
        }
    }

    private void LateUpdate()
    {
        // 1. Cámara moviéndose suavemente del Punto A al Punto B en el Menú Principal
        if (CurrentState == HubState.MainMenu && !isTransitioning && menuCamPointA != null && menuCamPointB != null)
        {
            menuPanTimer += Time.deltaTime * menuPanSpeed;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.PingPong(menuPanTimer, 1f));

            mainCamTransform.position = Vector3.Lerp(menuCamPointA.position, menuCamPointB.position, t);
            mainCamTransform.rotation = Quaternion.Slerp(menuCamPointA.rotation, menuCamPointB.rotation, t);
        }
        // 2. Cámara siguiendo al jugador 3D cuando ya pulsó Jugar
        else if (CurrentState == HubState.Exploring && !isTransitioning && playerController != null)
        {
            Vector3 desiredPosition = playerController.transform.position + cameraFollowOffset;
            mainCamTransform.position = Vector3.Lerp(mainCamTransform.position, desiredPosition, Time.deltaTime * followSmoothSpeed);

            Quaternion desiredRotation = Quaternion.Euler(cameraFollowRotation);
            mainCamTransform.rotation = Quaternion.Slerp(mainCamTransform.rotation, desiredRotation, Time.deltaTime * followSmoothSpeed);
        }
    }

    private IEnumerator FadeOutEntryBlackScreen()
    {
        float elapsed = 0f;
        Color c = blackFadeOverlay.color;

        while (elapsed < entryFadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, Mathf.Clamp01(elapsed / entryFadeDuration));
            blackFadeOverlay.color = c;
            yield return null;
        }

        c.a = 0f;
        blackFadeOverlay.color = c;
        blackFadeOverlay.gameObject.SetActive(false);
    }

    // --- BOTONES DEL MENÚ PRINCIPAL Y VOLVER ---

    public void StartGameFromMenu()
    {
        if (CurrentState != HubState.MainMenu || isTransitioning) return;

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        StartCoroutine(TransitionMenuToExplore());
    }

    public void ReturnToMainMenu()
    {
        if (CurrentState != HubState.Exploring || isTransitioning) return;

        if (returnToMenuButton != null) returnToMenuButton.SetActive(false);
        if (playerController != null) playerController.CanMove = false;

        StartCoroutine(TransitionExploreToMenu());
    }

    public void ToggleOptionsPanel(bool show)
    {
        if (optionsPanel != null) optionsPanel.SetActive(show);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(!show);
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private IEnumerator TransitionMenuToExplore()
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
            t += Time.deltaTime * (transitionSpeed * 0.75f);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            if (playerController != null)
            {
                targetPos = playerController.transform.position + cameraFollowOffset;
            }

            mainCamTransform.position = Vector3.Lerp(startPos, targetPos, smoothT);
            mainCamTransform.rotation = Quaternion.Slerp(startRot, targetRot, smoothT);
            yield return null;
        }

        CurrentState = HubState.Exploring;
        isTransitioning = false;

        if (playerController != null) playerController.CanMove = true;
        if (returnToMenuButton != null) returnToMenuButton.SetActive(true);
    }

    private IEnumerator TransitionExploreToMenu()
    {
        isTransitioning = true;
        float t = 0f;
        Vector3 startPos = mainCamTransform.position;
        Quaternion startRot = mainCamTransform.rotation;

        // Calculamos el punto exacto del recorrido A-B para enganchar sin cortes al llegar
        float panT = Mathf.SmoothStep(0f, 1f, Mathf.PingPong(menuPanTimer, 1f));
        Vector3 targetPos = (menuCamPointA != null && menuCamPointB != null)
            ? Vector3.Lerp(menuCamPointA.position, menuCamPointB.position, panT)
            : startPos;
        Quaternion targetRot = (menuCamPointA != null && menuCamPointB != null)
            ? Quaternion.Slerp(menuCamPointA.rotation, menuCamPointB.rotation, panT)
            : startRot;

        while (t < 1f)
        {
            t += Time.deltaTime * (transitionSpeed * 0.75f);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            mainCamTransform.position = Vector3.Lerp(startPos, targetPos, smoothT);
            mainCamTransform.rotation = Quaternion.Slerp(startRot, targetRot, smoothT);
            yield return null;
        }

        mainCamTransform.position = targetPos;
        mainCamTransform.rotation = targetRot;

        CurrentState = HubState.MainMenu;
        isTransitioning = false;

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    // --- NAVEGACIÓN ESTANTERÍA Y ATRIL ---

    public void EnterShelfView()
    {
        if (CurrentState != HubState.Exploring || isTransitioning) return;

        if (returnToMenuButton != null) returnToMenuButton.SetActive(false);
        if (playerController != null) playerController.CanMove = false;
        StartCoroutine(TransitionExploreToShelf());
    }

    public void ExitShelfView()
    {
        if (CurrentState != HubState.ShelfView || isTransitioning) return;

        if (exitShelfButton != null) exitShelfButton.SetActive(false);
        StartCoroutine(TransitionShelfToExplore());
    }

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
        if (returnToMenuButton != null) returnToMenuButton.SetActive(true);
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