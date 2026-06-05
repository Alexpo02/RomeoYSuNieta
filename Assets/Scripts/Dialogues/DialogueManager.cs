using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Singleton que gestiona la UI de diálogos y bloquea/desbloquea al jugador.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Referencias")]
    [Tooltip("Panel raíz de la caja de diálogo (el que se activa/desactiva)")]
    [SerializeField]
    private GameObject dialoguePanel;

    [SerializeField]
    private Canvas dialogueCanvas;

    [Tooltip("TextMeshPro para el nombre del personaje")]
    [SerializeField]
    private TextMeshProUGUI speakerNameText;

    [Tooltip("TextMeshPro para el texto del diálogo")]
    [SerializeField]
    private TextMeshProUGUI dialogueText;

    [Header("Referencias de jugador")]
    [SerializeField]
    private Player player;

    [SerializeField]
    private PlayerInteractor playerInteractor;

    [SerializeField]
    private PlayerInput playerInput;

    [Header("Typewriter")]
    [Tooltip("Segundos entre cada carácter")]
    [SerializeField]
    private float charDelay = 0.03f;

    // Estado interno
    private DialogueLine[] currentLines;
    private int currentIndex;
    private bool isActive;
    private bool isTyping; // ¿está la corrutina escribiendo ahora mismo?
    private Coroutine typingRoutine;

    private DialogueTrigger currentTrigger;

    private const string GAMEPLAY_MAP = "Player";

    // ─────────────────────────────────────────────
    // Ciclo de vida
    // ─────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        dialoguePanel.SetActive(false);
    }

    // ─────────────────────────────────────────────
    // API pública
    // ─────────────────────────────────────────────

    public void StartDialogue(DialogueData data, DialogueTrigger trigger = null)
    {
        if (isActive || data == null || data.lines.Length == 0)
            return;

        currentLines = data.lines;
        currentIndex = 0;
        isActive = true;
        currentTrigger = trigger;

        BlockPlayer(true);
        dialoguePanel.SetActive(true);
        if (dialogueCanvas != null)
            dialogueCanvas.gameObject.SetActive(true);

        ShowLine(currentIndex);
    }

    /// <summary>
    /// Llamado desde el input de diálogo (tecla E / botón A / etc.).
    /// - Si el typewriter está escribiendo  → muestra el texto completo al instante.
    /// - Si ya terminó de escribir          → avanza a la siguiente línea.
    /// </summary>
    public void OnAdvanceDialogue(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
            return;
        if (!isActive)
            return;

        if (isTyping)
        {
            // Completa la línea actual al instante
            SkipTypewriter();
            return;
        }

        // Avanza
        currentIndex++;
        if (currentIndex >= currentLines.Length)
            EndDialogue();
        else
            ShowLine(currentIndex);
    }

    public bool IsActive => isActive;

    // ─────────────────────────────────────────────
    // Typewriter
    // ─────────────────────────────────────────────

    private void ShowLine(int index)
    {
        speakerNameText.text = currentLines[index].speakerName;

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeLine(currentLines[index].text));
    }

    private IEnumerator TypeLine(string fullText)
    {
        isTyping = true;
        dialogueText.text = string.Empty;

        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(charDelay);
        }

        isTyping = false;
        typingRoutine = null;
    }

    private void SkipTypewriter()
    {
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }

        dialogueText.text = currentLines[currentIndex].text;
        isTyping = false;
    }

    // ─────────────────────────────────────────────
    // Fin de diálogo
    // ─────────────────────────────────────────────

    private void EndDialogue()
    {
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }

        isActive = false;
        isTyping = false;
        dialoguePanel.SetActive(false);
        if (dialogueCanvas != null)
            dialogueCanvas.gameObject.SetActive(false);
        BlockPlayer(false);
        currentLines = null;

        DialogueTrigger trigger = currentTrigger;
        currentTrigger = null;
        trigger?.NotifyDialogueEnd();
    }

    private void BlockPlayer(bool block)
    {
        if (playerInput == null)
            return;

        playerInput.SwitchCurrentActionMap(block ? "Dialogue" : GAMEPLAY_MAP);
    }
}
