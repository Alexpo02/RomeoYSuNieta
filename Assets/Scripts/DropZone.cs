using UnityEngine;

public class DropZone : MonoBehaviour, IInteractuable
{
    [Header("Configuración")]
    public string zoneName = "Zona de depósito";
    public string acceptedItemName = "";
    public string dropHereText = "Pulsa E para dejar aquí";
    public string wrongItemText = "Este objeto no va aquí";

    [Header("Puzzle padre (opcional)")]
    [SerializeField]
    private FurniturePuzzle puzzle;

    [Header("Bloqueo")]
    // Si true, una vez colocado el objeto correcto la zona se bloquea
    public bool lockOnCorrectPlacement = false;
    private bool isLocked = false;

    private Pickable placedItem = null;
    private PlayerInteractor playerInteractor;

    public bool IsOccupied => placedItem != null;
    public bool IsLocked => isLocked;
    public Pickable PlacedItem => placedItem;

    public void Lock()
    {
        isLocked = true;
    }

    private void Update()
    {
        // Si el objeto colocado ha sido cogido de nuevo, libera la zona
        if (placedItem != null && placedItem.IsHeld && !isLocked)
            placedItem = null;
    }

    public void Interact()
    {
        if (isLocked)
            return;

        if (playerInteractor == null)
            playerInteractor = FindFirstObjectByType<PlayerInteractor>();

        // Si hay objeto colocado y el jugador no lleva nada, lo coge
        if (IsOccupied && playerInteractor.HeldItem == null)
        {
            PickUpPlacedItem();
            return;
        }

        Pickable heldItem = playerInteractor.HeldItem;
        if (heldItem == null)
            return;

        if (IsOccupied)
        {
            Debug.Log($"[DropZone] {zoneName} ya está ocupada.");
            return;
        }

        /*if (!string.IsNullOrEmpty(acceptedItemName) && heldItem.itemName != acceptedItemName)
        {
            Debug.Log($"[DropZone] {wrongItemText}");
            return;
        }*/

        heldItem.PlaceAt(transform.position, transform.rotation);
        placedItem = heldItem;

        // Bloquea si está configurado para ello
        if (lockOnCorrectPlacement)
            isLocked = true;

        Debug.Log($"[DropZone] {heldItem.itemName} colocado en {zoneName}. Bloqueado: {isLocked}");
        OnItemPlaced(placedItem);
    }

    private void PickUpPlacedItem()
    {
        if (playerInteractor == null)
            return;
        Pickable item = placedItem;
        placedItem = null; // limpia la zona antes de coger
        item.PickUp(playerInteractor);
        Debug.Log($"[DropZone] {item.itemName} recogido de {zoneName}");
    }

    protected virtual void OnItemPlaced(Pickable item)
    {
        puzzle?.CheckPuzzle();
    }

    public void GetInteractionText()
    {
        if (playerInteractor == null)
            playerInteractor = FindFirstObjectByType<PlayerInteractor>();

        if (isLocked)
            return;

        if (IsOccupied && playerInteractor.HeldItem == null)
            Debug.Log($"[UI] Pulsa E para coger: {placedItem.itemName}");
        else if (!IsOccupied && playerInteractor.HeldItem != null)
            Debug.Log($"[UI] {dropHereText}");
    }

    public void HideText()
    {
        Debug.Log("[UI] Ocultar texto");
    }
}
