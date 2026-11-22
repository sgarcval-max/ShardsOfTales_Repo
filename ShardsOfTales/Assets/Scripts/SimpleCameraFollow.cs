using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(1.5f, 2.5f, -10f);
    public float smoothSpeed = 6f;

    [HideInInspector] public bool isOverrideActive = false;

    private void LateUpdate()
    {
        if (target == null || isOverrideActive) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * smoothSpeed);
    }
}
