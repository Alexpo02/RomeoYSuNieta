using UnityEngine;

/// <summary>
/// Componente que convierte un GameObject (cubo) en un reloj interactuable.
/// Implementa IInteractuable para integrarse con PlayerInteractor.
/// </summary>
public class ClockInteractable : MonoBehaviour, IInteractuable
{
    [Header("Referencias")]
    [Tooltip("Canvas del reloj que se mostrará al interactuar")]
    public ClockUI clockUI;

    [Header("Texto de interacción")]
    [Tooltip("Referencia al componente que muestra el texto de interacción (opcional)")]
    public InteractionTextDisplay interactionTextDisplay;

    [Tooltip("Mensaje que aparece al acercarse al reloj")]
    public string interactionText = "Pulsa E para examinar el reloj";

    // ─── IInteractuable ───────────────────────────────────────────────────────

    public void Interact()
    {
        if (clockUI != null)
            clockUI.Show();
        else
            Debug.LogWarning("[ClockInteractable] ClockUI no asignado.");
    }

    public void GetInteractionText()
    {
        if (interactionTextDisplay != null)
            interactionTextDisplay.ShowText(interactionText);
        else
            Debug.Log($"[ClockInteractable] {interactionText}");
    }

    public void HideText()
    {
        if (interactionTextDisplay != null)
            interactionTextDisplay.HideText();
    }
}
