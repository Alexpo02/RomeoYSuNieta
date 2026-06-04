using UnityEngine;

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
        OnStart, // Se lanza automáticamente al arrancar la escena
        OnInteract, // El jugador pulsa E al estar en rango (usa IInteractuable)
        OnTriggerEnter, // El jugador entra en un trigger de zona (pasillo)
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
        TryPlay();
    }

    public void GetInteractionText()
    {
        if (triggerMode != TriggerMode.OnInteract)
            return;
        // Aquí conecta con tu sistema de UI de hint si lo tienes
        Debug.Log($"[DialogueTrigger] Hint: {interactionHint}");
    }

    public void HideText()
    {
        // Oculta el hint de interacción si lo tienes en UI
    }

    // ─────────────────────────────────────────────
    // Lógica común
    // ─────────────────────────────────────────────

    private void TryPlay()
    {
        if (playOnce && hasPlayed)
            return;

        DialogueManager.Instance.StartDialogue(dialogueData);
        hasPlayed = true;
    }
}
