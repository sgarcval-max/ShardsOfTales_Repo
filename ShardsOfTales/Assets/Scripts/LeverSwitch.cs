using UnityEngine;

public class LeverSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private Door targetDoor;
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Material inactiveMaterial;

    private bool isActivated = false;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateVisuals();
    }

    public void Interact()
    {
        isActivated = !isActivated;

        if (targetDoor != null)
        {
            targetDoor.ToggleDoor();
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (meshRenderer == null) return;

        if (isActivated && activeMaterial != null)
        {
            meshRenderer.material = activeMaterial;
        }
        else if (!isActivated && inactiveMaterial != null)
        {
            meshRenderer.material = inactiveMaterial;
        }
    }
}