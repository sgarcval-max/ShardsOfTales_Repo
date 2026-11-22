using UnityEngine;

public class LevelPlayer25D : MonoBehaviour
{
    [Header("Movimiento y Salto")]
    public float moveSpeed = 7f;
    public float jumpForce = 11f;
    public Transform groundCheck;
    public Transform startPoint;

    [Header("Interacción")]
    public float interactRadius = 2.2f;

    private Rigidbody rb;
    private bool isGrounded;
    private float moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        bool dialogueOpen = SimpleDialogueUI.Instance != null && SimpleDialogueUI.Instance.IsOpen;
        bool ocarinaOpen = OcarinaMinigameUI.Instance != null && OcarinaMinigameUI.Instance.IsOpen;
        bool riverPlaying = RiverCrossyMinigame.Instance != null && RiverCrossyMinigame.Instance.IsPlaying;
        bool rocksOpen = CaveRocksMinigameUI.Instance != null && CaveRocksMinigameUI.Instance.IsOpen;

        if (dialogueOpen || ocarinaOpen || riverPlaying || rocksOpen)
        {
            moveInput = 0f;
            return;
        }

        // 1. Movimiento lateral (A / D)
        moveInput = Input.GetAxisRaw("Horizontal");

        // 2. Comprobar suelo
        if (groundCheck != null)
        {
            int mask = ~LayerMask.GetMask("Player");
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.25f, mask, QueryTriggerInteraction.Ignore);
        }

        // 3. Salto con Espacio
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0f);
        }

        // 4. Interactuar con tecla E o Clic Izquierdo
        if (Input.GetKeyDown(KeyCode.E))
        {
            InteractNearby();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            InteractWithMouse();
        }

        // 5. Si cae al vacío, vuelve al inicio
        if (transform.position.y < -8f && startPoint != null)
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = startPoint.position;
        }
    }

    private void FixedUpdate()
    {
        if (RiverCrossyMinigame.Instance != null && RiverCrossyMinigame.Instance.IsPlaying) return;
        rb.linearVelocity = new Vector3(moveInput * moveSpeed, rb.linearVelocity.y, 0f);
    }

    private void InteractNearby()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRadius);
        foreach (Collider col in hits)
        {
            IInteractable item = col.GetComponentInParent<IInteractable>();
            if (item != null)
            {
                item.Interact();
                return;
            }
        }
    }

    private void InteractWithMouse()
    {
        if (Camera.main == null) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 40f))
        {
            IInteractable item = hit.collider.GetComponentInParent<IInteractable>();
            if (item != null)
            {
                item.Interact();
            }
        }
    }
}