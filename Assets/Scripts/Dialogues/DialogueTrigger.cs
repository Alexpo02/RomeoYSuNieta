using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Lanza un DialogueData. Reutilizable para tres casos:
///   1. Interactuable (NPC)   → implementa IInteractuable, triggerMode = OnInteract
///   2. Trigger de zona       → triggerMode = OnTriggerEnter (pasillo)
///   3. Al inicio del juego   → triggerMode = OnStart
/// </summary>
public class DialogueTrigger : MonoBehaviour, IInteractuable
{
    public enum TriggerMode
    {
        OnStart,
        OnInteract,
        OnTriggerEnter,
    }

    [Header("Datos del diálogo")]
    [SerializeField]
    private DialogueData dialogueData;

    [Header("Modo de activación")]
    [SerializeField]
    private TriggerMode triggerMode = TriggerMode.OnInteract;

    [Header("Solo para OnInteract")]
    [Tooltip("Texto que aparece en pantalla indicando que se puede interactuar")]
    [SerializeField]
    private string interactionHint = "Pulsa E para hablar";

    [Header("Opciones")]
    [Tooltip("Si está activo, el diálogo solo se reproduce una vez")]
    [SerializeField]
    private bool playOnce = true;

    [Header("Evento al terminar el diálogo")]
    [Tooltip(
        "Se invoca cuando termina este diálogo. Úsalo para mostrar un canvas, activar objetos, etc."
    )]
    [SerializeField]
    private UnityEvent onDialogueEnd;
    
    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip crowsound;


    private bool hasPlayed;

    // ─────────────────────────────────────────────
    // Modos automáticos
    // ─────────────────────────────────────────────

    private void Start()
    {
        if (triggerMode == TriggerMode.OnStart)
            TryPlay();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerMode != TriggerMode.OnTriggerEnter)
            return;
        if (!other.CompareTag("Player"))
            return;
        TryPlay();
    }

    // ─────────────────────────────────────────────
    // IInteractuable (modo OnInteract)
    // ─────────────────────────────────────────────

    public void Interact()
    {
        if (triggerMode != TriggerMode.OnInteract)
            return;
                            if (audioSource != null && crowsound != null)
                audioSource.PlayOneShot(crowsound);
        TryPlay();
    }

    public void GetInteractionText()
    {
        if (triggerMode != TriggerMode.OnInteract)
            return;
        Debug.Log($"[DialogueTrigger] Hint: {interactionHint}");
    }

    public void HideText() { }

    // ─────────────────────────────────────────────
    // API pública
    // ─────────────────────────────────────────────

    /// <summary>
    /// Llamado por DialogueManager cuando termina el diálogo que este trigger inició.
    /// </summary>
    public void NotifyDialogueEnd()
    {
        onDialogueEnd?.Invoke();
    }

    /// <summary>
    /// Permite que el diálogo vuelva a reproducirse (usado por Cage al abrirse).
    /// </summary>
    public void ResetDialogue()
    {
        hasPlayed = false;
    }

    // ─────────────────────────────────────────────
    // Lógica común
    // ─────────────────────────────────────────────

    private void TryPlay()
    {

        if (playOnce && hasPlayed)
            return;
                if (audioSource != null && crowsound != null)
                audioSource.PlayOneShot(crowsound);
        DialogueManager.Instance.StartDialogue(dialogueData, this);
        hasPlayed = true;
    }
}
