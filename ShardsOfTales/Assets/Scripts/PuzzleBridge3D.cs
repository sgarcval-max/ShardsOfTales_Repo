using UnityEngine;

public class PuzzleBridge3D : MonoBehaviour
{
    [Header("Posiciones del Puente")]
    [SerializeField] private Vector3 hiddenOffset = new Vector3(0f, -4f, 0f);
    [SerializeField] private float moveSpeed = 4f;

    private Vector3 activePosition;
    private Vector3 targetPosition;
    private bool isActivated = false;

    private void Awake()
    {
        // La posición donde lo colocas en el editor será su posición desplegada
        activePosition = transform.position;
        transform.position = activePosition + hiddenOffset;
        targetPosition = transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
    }

    public void ActivateBridge()
    {
        if (isActivated) return;
        isActivated = true;
        targetPosition = activePosition;
    }
}