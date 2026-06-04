using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Objeto sostenido")]
    public Transform holdPoint;
    public Pickable HeldItem { get; set; }

    private IInteractuable interactuableActual;
    private InteractableHighlight currentHighlight; // ← referencia al highlight activo

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

        interactuableActual.Interact();
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

            // Flash blanco si el objeto tiene el componente
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

            // Restaurar color al salir
            currentHighlight?.ResetColor();
            currentHighlight = null;
        }
    }
}
