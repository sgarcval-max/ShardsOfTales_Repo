using UnityEngine;

public class PlayerController3D : MonoBehaviour
{
    [Header("Movimiento 3D")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private Transform visuals3D; // Arrastra aquí el hijo Visuals_3D

    [Header("Salto")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.25f;

    [Header("Interacción")]
    [SerializeField] private float interactRange = 15f;

    private Rigidbody rb;
    private Vector3 moveInput;
    private bool isGrounded;

    // Se pondrá en false cuando hagamos zoom a la estantería para elegir libro
    public bool CanMove { get; set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!CanMove)
        {
            moveInput = Vector3.zero;
            return;
        }

        // 1. Lectura de movimiento en plano 3D (X y Z)
        float x = Input.GetAxisRaw("Horizontal"); // A / D
        float z = Input.GetAxisRaw("Vertical");   // W / S
        moveInput = new Vector3(x, 0f, z).normalized;

        // 2. Rotación suave del modelo 3D hacia la dirección en la que camina
        if (moveInput != Vector3.zero && visuals3D != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            visuals3D.rotation = Quaternion.Slerp(visuals3D.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // 3. Comprobar suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // 4. Salto (Espacio)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }

        // 5. Interacción con Clic Izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            InteractRaycast();
        }
    }

    private void FixedUpdate()
    {
        if (!CanMove)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, moveInput.z * moveSpeed);
    }

    private void InteractRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}