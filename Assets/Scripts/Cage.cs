using UnityEngine;

/// <summary>
/// Jaula interactuable con dos comportamientos:
///   - Sin llave: lanza el diálogo "lockedDialogue" (el pájaro pide que lo liberen)
///   - Con llave:  abre la jaula y prepara el DialogueTrigger del pájaro libre
/// </summary>
public class Cage : MonoBehaviour, IInteractuable
{
    [Tooltip("Debe coincidir exactamente con el keyId de la llave correspondiente")]
    [SerializeField]
    private string keyId;

    [Header("Diálogos")]
    [Tooltip("Diálogo que aparece al interactuar sin tener la llave (ej: '¡Ábreme!')")]
    [SerializeField]
    private DialogueData lockedDialogue;

    [Header("Referencias")]
    [Tooltip("Puerta de la jaula (script Door en el objeto bisagra)")]
    [SerializeField]
    private Door cageDoor; // ← nueva referencia

    [Tooltip("Collider de la jaula (normalmente este mismo objeto)")]
    [SerializeField]
    private Collider cageCollider;

    [Tooltip("Collider del pájaro (empieza desactivado)")]
    [SerializeField]
    private Collider birdCollider;

    [Tooltip("DialogueTrigger del pájaro libre (para resetearlo al abrir la jaula)")]
    [SerializeField]
    private DialogueTrigger birdDialogueTrigger;

    private bool isOpen;

        [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip crowsound,cageopen;

    public void Interact()
    {
        if (isOpen)
            return;

        if (!KeyInventory.Instance.HasKey(keyId))
        {
            if (lockedDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(lockedDialogue);

                if (audioSource != null && crowsound != null)
                audioSource.PlayOneShot(crowsound);
            }
            else
                Debug.Log($"[Cage] Necesitas la llave '{keyId}' para abrir esta jaula.");
            return;
        }
        if (audioSource != null && cageopen != null)
        audioSource.PlayOneShot(cageopen);
        OpenCage();
    }

    public void GetInteractionText() { }

    public void HideText() { }

    private void OpenCage()
    {
        isOpen = true;

        cageDoor?.Interact(); // ← abre la puerta con su animación

        cageCollider.enabled = false;
        birdCollider.enabled = true;
        birdDialogueTrigger?.ResetDialogue();

        Debug.Log($"[Cage] Jaula '{keyId}' abierta.");
    }
}
