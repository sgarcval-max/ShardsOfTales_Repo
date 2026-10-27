using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private Vector3 openOffset = new Vector3(0f, 4f, 0f);
    [SerializeField] private float speed = 3f;

    private Vector3 closedPosition;
    private Vector3 targetPosition;
    private bool isOpen = false;

    private void Start()
    {
        closedPosition = transform.position;
        targetPosition = closedPosition;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        targetPosition = isOpen ? closedPosition + openOffset : closedPosition;
    }
}
