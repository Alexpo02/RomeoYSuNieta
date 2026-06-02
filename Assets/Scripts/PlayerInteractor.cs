using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    private IInteractuable interactuableActual;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (interactuableActual == null)
        {
            //Debug.Log("No hay interactuable actual");
            return;
        }

        interactuableActual.Interact();
        interactuableActual = null;
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger Enter");

        IInteractuable i = other.GetComponent<IInteractuable>();

        if (i != null)
        {
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