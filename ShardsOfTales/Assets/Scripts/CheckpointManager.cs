using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Vector3 currentCheckpointPosition;
    private bool checkpointSet = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckpoint(Vector3 newPosition)
    {
        currentCheckpointPosition = newPosition;
        checkpointSet = true;
        Debug.Log("�Punto de control alcanzado en: " + newPosition);
    }

    public void RespawnPlayer(GameObject player)
    {
        if (checkpointSet)
        {
            // Desactivamos el CharacterController o Rigidbody temporalmente si interfiere con la teleportaci�n
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
            }

            player.transform.position = currentCheckpointPosition;
            Debug.Log("Jugador reaparecido en el �ltimo Checkpoint.");
        }
        else
        {
            // Si no hay checkpoint activo, recarga la escena actual
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}
