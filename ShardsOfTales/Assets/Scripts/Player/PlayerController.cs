using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento Horizontal")]
    [SerializeField] private float moveSpeed = 8f;
    private float horizontalInput;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    private bool isGrounded;

    [Header("Dash")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Si est� haciendo Dash, ignoramos el resto de controles temporalmente
        if (isDashing) return;

        // Movimiento Horizontal (A / D)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Detecci�n de Suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Salto (Espacio)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // Dash (Tecla F)
        if (Input.GetKeyDown(KeyCode.F) && canDash)
        {
            StartCoroutine(PerformDash());
        }

        // Interacci�n (Clic Izquierdo)
        if (Input.GetMouseButtonDown(0))
        {
            Interact();
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        // Aplicar movimiento en el eje X manteniendo la velocidad vertical (Y) y bloqueando la Z
        rb.linearVelocity = new Vector3(horizontalInput * moveSpeed, rb.linearVelocity.y, 0f);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0f);
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.useGravity ? 1f : 0f;
        rb.useGravity = false;

        // Determina la direcci�n del dash seg�n el movimiento o hacia d�nde mira
        float dashDirection = horizontalInput != 0 ? Mathf.Sign(horizontalInput) : 1f;
        rb.linearVelocity = new Vector3(dashDirection * dashForce, 0f, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.useGravity = true;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void Interact()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 15f))
        {
            // Comprobamos si el objeto tocado se puede interactuar
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar el radio del Ground Check en la ventana Scene
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
