using UnityEngine;

public class EstanteriaInteract : MonoBehaviour
{
    public CamaraHub camara;
    public GameObject avisoInteractuar;

    private bool playerCerca;

    void Start()
    {
        avisoInteractuar.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerCerca = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerCerca = false;
    }

    void Update()
    {
        bool puede = playerCerca && camara.PuedeEntrarEstanteria();
        avisoInteractuar.SetActive(puede);

        if (puede && Input.GetKeyDown(KeyCode.E))
        {
            avisoInteractuar.SetActive(false);
            camara.EntrarEstanteria();
        }
    }
}
