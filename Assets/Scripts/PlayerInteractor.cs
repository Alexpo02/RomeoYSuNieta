using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Objeto sostenido")]
    public Transform holdPoint;
    public Pickable HeldItem { get; set; }

    private IInteractuable interactuableActual;

    public void OnInteract(InputAction.CallbackContext context)
    {
        // Con Invoke Unity Events filtramos así
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
        }
    }

    void OnTriggerExit(Collider other)
    {
        IInteractuable i = other.GetComponent<IInteractuable>();
        if (i != null && i == interactuableActual)
        {
            i.HideText();
            interactuableActual = null;
        }
    }
}
