using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Punto de Inicio del Nivel")]
    [SerializeField] private Transform levelStartPoint;
    [SerializeField] private Transform playerTransform;

    private Rigidbody playerRb;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        if (playerTransform != null)
        {
            playerRb = playerTransform.GetComponent<Rigidbody>();
        }
    }

    // Devuelve al jugador al inicio del cuento cuando toca pinchos o cae al vacío
    public void RespawnPlayer()
    {
        if (playerTransform == null || levelStartPoint == null) return;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        playerTransform.position = levelStartPoint.position;
    }
}
