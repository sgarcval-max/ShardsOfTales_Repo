using UnityEngine;

public class SpriteBillboard25D : MonoBehaviour
{
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

        // Mantiene el sprite 2D mirando siempre de frente a la cámara (incluyendo la vista cenital del río)
        transform.rotation = mainCam.transform.rotation;
    }
}
