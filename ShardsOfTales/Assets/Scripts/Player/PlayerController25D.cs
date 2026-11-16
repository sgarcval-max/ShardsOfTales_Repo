using System.Collections;
using UnityEngine;

public class PlayerController25D : MonoBehaviour
{
    [Header("Modo de Movimiento")]
    [Tooltip("Déjalo desmarcado en los niveles para moverte solo en lateral (A/D).")]
    [SerializeField] private bool allowDepthMovement = false;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private Transform spriteVisuals; // Arrastra aquí el hijo Visuals_Sprite
    private float moveX;
    private float moveZ;
    private bool facingRight = true;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 11f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.25f;
    private bool isGrounded;

    [Header("Dash")]
    [SerializeField] private float dashForce = 18f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;

    [Header("Interacción")]
    [SerializeField] private float interactRange = 12f;

    private Rigidbody rb;
    public bool CanMove { get; set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!CanMove)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        if (isDashing) return;

        // 1. Lectura de controles (A / D)
        moveX = Input.GetAxisRaw("Horizontal");
        moveZ = allowDepthMovement ? Input.GetAxisRaw("Vertical") : 0f;

        // 2. Girar el Sprite 2D sin rotar el Collider 3D
        if (moveX > 0 && !facingRight) FlipSprite();
        else if (moveX < 0 && facingRight) FlipSprite();

        // 3. Detectar si toca el suelo 3D
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // 4. Salto (Tecla Espacio)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }

        // 5. Dash (Tecla F)
        if (Input.GetKeyDown(KeyCode.F) && canDash)
        {
            StartCoroutine(PerformDash());
        }

        // 6. Interacción (Clic Izquierdo del ratón)
        if (Input.GetMouseButtonDown(0))
        {
            InteractRaycast();
        }
    }

    private void FixedUpdate()
    {
        if (!CanMove || isDashing) return;

        Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;
        rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);

        // Congelamos la profundidad Z en los niveles para que el personaje no se salga del carril
        if (!allowDepthMovement)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        rb.useGravity = false;

        Vector3 dashDir = new Vector3(moveX, 0f, moveZ).normalized;
        if (dashDir == Vector3.zero)
        {
            dashDir = facingRight ? Vector3.right : Vector3.left;
        }

        rb.linearVelocity = dashDir * dashForce;

        yield return new WaitForSeconds(dashDuration);

        rb.useGravity = true;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void FlipSprite()
    {
        facingRight = !facingRight;
        if (spriteVisuals != null)
        {
            Vector3 scale = spriteVisuals.localScale;
            scale.x *= -1f;
            spriteVisuals.localScale = scale;
        }
    }

    private void InteractRaycast()
    {
        if (Camera.main == null) return;

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
