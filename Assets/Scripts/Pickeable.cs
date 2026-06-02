using UnityEngine;

public class Pickable : MonoBehaviour, IInteractuable
{
    [Header("Configuraci�n")]
    public string itemName = "Objeto";
    public string pickUpText = "Pulsa E para coger";
    public string dropText = "Pulsa E para soltar";

    private bool isHeld = false;
    private PlayerInteractor playerInteractor;

    // Layer que usar�n los objetos cogidos para ignorar al jugador
    // Crea un layer "HeldItem" en Unity y ponlo aqu�, o usa el n�mero directamente
    [SerializeField]
    private string heldItemLayer = "HeldItem";
    private int originalLayer;

    public bool IsHeld => isHeld;

    public void Interact()
    {
        if (!isHeld)
            TryPickUp();
        /*else
            Drop();*/
    }

    private void TryPickUp()
    {
        if (playerInteractor == null)
            playerInteractor = FindFirstObjectByType<PlayerInteractor>();
        if (playerInteractor.HeldItem != null)
            return;
        PickUp(playerInteractor);
    }

    public void PickUp(PlayerInteractor interactor)
    {
        playerInteractor = interactor;
        playerInteractor.HeldItem = this;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            //rb.linearVelocity = Vector3.zero;
        }

        // Desactiva TODOS los colliders del libro al cogerlo
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;

        originalLayer = gameObject.layer;
        int newLayer = LayerMask.NameToLayer(heldItemLayer);
        if (newLayer != -1)
            SetLayerRecursively(gameObject, newLayer);

        Transform parent =
            interactor.holdPoint != null ? interactor.holdPoint : interactor.transform;

        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        isHeld = true;
        Debug.Log($"[Pickable] Cogido: {itemName}");
    }

    public void Drop()
    {
        isHeld = false;

        if (playerInteractor != null)
            playerInteractor.HeldItem = null;

        transform.SetParent(null);
        SetLayerRecursively(gameObject, originalLayer);

        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        playerInteractor = null;
    }

    public void PlaceAt(Vector3 position, Quaternion rotation)
    {
        // Primero Drop sin reactivar física, luego posicionamos
        isHeld = false;

        if (playerInteractor != null)
            playerInteractor.HeldItem = null;

        transform.SetParent(null);
        SetLayerRecursively(gameObject, originalLayer);

        // Mantenemos isKinematic=true para que no caiga al colocarlo
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = true;

        transform.position = position;
        transform.rotation = rotation;

        playerInteractor = null;
        Debug.Log($"[Pickable] Colocado en: {position}");
    }

    public void GetInteractionText()
    {
        string text = isHeld ? dropText : pickUpText;
        Debug.Log($"[UI] {text}: {itemName}");
    }

    public void HideText()
    {
        Debug.Log("[UI] Ocultar texto");
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
