using UnityEngine;

public class EstanteriaInteract : MonoBehaviour
{
    public GameObject panelLibros;
    public PlayerHubMovement playerMovement;

    private bool playerCerca;
    private bool menuAbierto;

    void Start()
    {
        if (panelLibros != null)
            panelLibros.SetActive(false);
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
        if (!menuAbierto && playerCerca && Input.GetKeyDown(KeyCode.E))
        {
            AbrirMenu();
        }
        else if (menuAbierto && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)))
        {
            CerrarMenu();
        }
    }

    public void AbrirMenu()
    {
        menuAbierto = true;
        panelLibros.SetActive(true);
        playerMovement.enabled = false;
    }

    public void CerrarMenu()
    {
        menuAbierto = false;
        panelLibros.SetActive(false);
        playerMovement.enabled = true;
    }
}
