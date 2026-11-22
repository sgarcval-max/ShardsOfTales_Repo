using UnityEngine;

public class ForestDropZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && PiperQuestManager.Instance != null)
        {
            PiperQuestManager.Instance.DeliverFollowingRatsToForest();
        }
    }
}