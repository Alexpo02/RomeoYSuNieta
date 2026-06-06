using UnityEngine;

public class PaperInteractable : MonoBehaviour, IInteractuable
{
    [Header("Referencias")]
    public PaperUI paperUI;
    public InteractionTextDisplay interactionTextDisplay;

    [Header("Texto de interacción")]
    public string interactionText = "Pulsa E para leer la nota";

    public void Interact()
    {
        if (paperUI != null)
            paperUI.Show();
        else
            Debug.LogWarning("[PaperInteractable] PaperUI no asignado.");
    }

    public void GetInteractionText()
    {
        if (interactionTextDisplay != null)
            interactionTextDisplay.ShowText(interactionText);
        else
            Debug.Log($"[PaperInteractable] {interactionText}");
    }

    public void HideText()
    {
        if (interactionTextDisplay != null)
            interactionTextDisplay.HideText();
    }
}
