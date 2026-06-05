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

    // ─────────────────────────────────────────────
    // IInteractuable
    // ─────────────────────────────────────────────

    public void Interact()
    {
        if (isOpen)
            return;

        if (!KeyInventory.Instance.HasKey(keyId))
        {
            // Sin llave: lanza el diálogo del pájaro encerrado
            if (lockedDialogue != null)
                DialogueManager.Instance.StartDialogue(lockedDialogue);
            else
                Debug.Log($"[Cage] Necesitas la llave '{keyId}' para abrir esta jaula.");
            return;
        }

        OpenCage();
    }

    public void GetInteractionText() { }

    public void HideText() { }

    // ─────────────────────────────────────────────
    // Lógica interna
    // ─────────────────────────────────────────────

    private void OpenCage()
    {
        isOpen = true;
        cageCollider.enabled = false;
        birdCollider.enabled = true;
        birdDialogueTrigger.ResetDialogue();

        Debug.Log($"[Cage] Jaula '{keyId}' abierta.");
    }
}
