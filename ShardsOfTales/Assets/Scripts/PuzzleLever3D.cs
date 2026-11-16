using UnityEngine;

public class PuzzleLever3D : MonoBehaviour, IInteractable
{
    [Header("Conexión con el Puzle")]
    [SerializeField] private PuzzleBridge3D targetBridge;
    [SerializeField] private Transform leverStick; // Pieza visual que rotará al activarla
    [SerializeField] private float maxInteractDistance = 5f;

    private Transform playerTransform;
    private bool isUsed = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    public void Interact()
    {
        if (isUsed) return;

        // Comprobamos que el bibliotecario esté cerca de la palanca
        if (playerTransform != null)
        {
            float dist = Vector3.Distance(playerTransform.position, transform.position);
            if (dist > maxInteractDistance)
            {
                Debug.Log("Acércate más a la palanca para activarla.");
                return;
            }
        }

        isUsed = true;

        // Giramos visualmente la palanca
        if (leverStick != null)
        {
            leverStick.localRotation = Quaternion.Euler(0f, 0f, -40f);
        }

        // Desplegamos el puente
        if (targetBridge != null)
        {
            targetBridge.ActivateBridge();
        }
    }
}
