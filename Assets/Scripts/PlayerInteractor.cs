using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Objeto sostenido")]
    public Transform holdPoint;
    public Pickable HeldItem { get; set; }

    private IInteractuable interactuableActual;
    private GameObject currentInteractableGO;
    private InteractableHighlight currentHighlight;
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
            return;

        if (interactuableActual == null)
        {
            if (HeldItem != null)
                HeldItem.Drop();
            return;
        }

        // ¿Usa animación de recoger?
        bool usePickupAnim =
            currentInteractableGO != null
            && currentInteractableGO.GetComponent<UsesPickupAnimation>() != null
            && player != null
            && player.CanPlayInteractAnimation;

        // ¿Usa animación de interactuar?
        bool useInteractAnim =
            currentInteractableGO != null
            && currentInteractableGO.GetComponent<UsesInteractAnimation>() != null
            && player != null
            && player.CanPlayInteractAnimation;

        if (usePickupAnim)
        {
            player.PlayPickupAnimation(() => interactuableActual?.Interact());
        }
        else if (useInteractAnim)
        {
            player.PlayInteractAnimation(() => interactuableActual?.Interact());
        }
        else
        {
            interactuableActual.Interact();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        IInteractuable i = other.GetComponent<IInteractuable>();
        if (i != null)
        {
            i.GetInteractionText();
            interactuableActual = i;
            currentInteractableGO = other.gameObject;

            currentHighlight = other.GetComponent<InteractableHighlight>();
            currentHighlight?.TriggerFlash();
        }
    }

    void OnTriggerExit(Collider other)
    {
        IInteractuable i = other.GetComponent<IInteractuable>();
        if (i != null && i == interactuableActual)
        {
            i.HideText();
            interactuableActual = null;
            currentInteractableGO = null;

            currentHighlight?.ResetColor();
            currentHighlight = null;
        }
    }
}
