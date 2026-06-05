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

        bool useAnim =
            currentInteractableGO != null
            && currentInteractableGO.GetComponent<UsesInteractAnimation>() != null
            && player != null
            && player.CanPlayInteractAnimation;

        if (useAnim)
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
        Debug.Log(
            $"[PlayerInteractor] TriggerEnter con: {other.gameObject.name}, padre: {other.transform.parent?.name}"
        );

        IInteractuable i = other.GetComponent<IInteractuable>();
        if (i != null)
        {
            Debug.Log($"[PlayerInteractor] Interactuable encontrado: {other.gameObject.name}");
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
