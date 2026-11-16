using UnityEngine;

public class BookshelfInteractable : MonoBehaviour, IInteractable
{
    [Header("Distancia máxima para interactuar")]
    [SerializeField] private float maxInteractDistance = 7f;
    [SerializeField] private Transform playerTransform;

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }
    }

    public void Interact()
    {
        if (LevelSelectManager.Instance == null || LevelSelectManager.Instance.CurrentState != LevelSelectManager.HubState.Exploring)
            return;

        // Comprobamos que el jugador esté lo bastante cerca de la estantería
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(playerTransform.position, transform.position);
            if (distance > maxInteractDistance)
            {
                Debug.Log("Acércate más a la estantería para examinar los libros.");
                return;
            }
        }

        LevelSelectManager.Instance.EnterShelfView();
    }
}
