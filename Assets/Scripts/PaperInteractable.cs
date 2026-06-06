using UnityEngine;

public class PaperInteractable : MonoBehaviour, IInteractuable
{
    [Header("Referencias")]
    public PaperUI paperUI;
    public InteractionTextDisplay interactionTextDisplay;

    [Header("Texto de interacción")]
    public string interactionText = "Pulsa E para leer la nota";

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip noteSound;

    public void Interact()
    {
        if (paperUI != null)
        {
            if (audioSource != null && noteSound != null)
                audioSource.PlayOneShot(noteSound);
            paperUI.Show();
        }
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

    public void CloseCanvas()
    {
        paperUI?.Hide();
    }
}
