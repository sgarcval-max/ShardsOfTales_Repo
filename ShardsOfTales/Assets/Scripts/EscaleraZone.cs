using UnityEngine;

public class EscaleraZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PlayerPlatformer2D player = other.GetComponent<PlayerPlatformer2D>();
        if (player != null) player.SetOnLadder(true);
    }

    void OnTriggerExit(Collider other)
    {
        PlayerPlatformer2D player = other.GetComponent<PlayerPlatformer2D>();
        if (player != null) player.SetOnLadder(false);
    }
}
