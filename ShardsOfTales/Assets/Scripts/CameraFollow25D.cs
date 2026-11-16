using UnityEngine;

public class CameraFollow25D : MonoBehaviour
{
    [Header("Objetivo a Seguir")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(2f, 2.5f, -10f);
    [SerializeField] private float smoothSpeed = 6f;

    private void Start()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
    }
}
