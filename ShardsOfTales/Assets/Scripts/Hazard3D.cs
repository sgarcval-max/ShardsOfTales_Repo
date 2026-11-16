using UnityEngine;

public class Hazard3D : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.RespawnPlayer();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.RespawnPlayer();
            }
        }
    }
}
