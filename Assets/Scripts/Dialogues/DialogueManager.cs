using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Singleton que gestiona la UI de diálogos y bloquea/desbloquea al jugador.
/// Necesita referencias a los componentes de UI asignados desde el Inspector.
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
    private PlayerInput playerInput; // componente PlayerInput del jugador

    // Estado interno
    private DialogueLine[] currentLines;
    private int currentIndex;
    private bool isActive;

    // Nombre de la action map de juego (la que tiene Move, Run, Interact...)
    private const string GAMEPLAY_MAP = "Player"; // cámbialo si tu map tiene otro nombre

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

    /// <summary>
    /// Inicia un diálogo. Bloquea al jugador y muestra la primera línea.
    /// Si ya hay un diálogo activo lo ignora.
    /// </summary>
    public void StartDialogue(DialogueData data)
    {
        if (isActive || data == null || data.lines.Length == 0)
            return;

        currentLines = data.lines;
        currentIndex = 0;
        isActive = true;

        BlockPlayer(true);
        dialoguePanel.SetActive(true);
        dialogueCanvas.gameObject.SetActive(true);
        ShowLine(currentIndex);
    }

    /// <summary>
    /// Avanza a la siguiente línea. Se llama desde el input de diálogo (tecla E).
    /// </summary>
    public void OnAdvanceDialogue(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
            return;
        if (!isActive)
            return;

        currentIndex++;

        if (currentIndex >= currentLines.Length)
        {
            EndDialogue();
        }
        else
        {
            ShowLine(currentIndex);
        }
    }

    public bool IsActive => isActive;

    // ─────────────────────────────────────────────
    // Métodos privados
    // ─────────────────────────────────────────────

    private void ShowLine(int index)
    {
        speakerNameText.text = currentLines[index].speakerName;
        dialogueText.text = currentLines[index].text;
    }

    private void EndDialogue()
    {
        isActive = false;
        dialoguePanel.SetActive(false);
        BlockPlayer(false);
        currentLines = null;
        dialogueCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// Bloquea o desbloquea el movimiento del jugador cambiando el action map activo.
    /// Durante el diálogo solo el action map "Dialogue" (con la E) está activo.
    /// </summary>
    private void BlockPlayer(bool block)
    {
        if (playerInput == null)
            return;

        if (block)
        {
            playerInput.SwitchCurrentActionMap("Dialogue");
        }
        else
        {
            playerInput.SwitchCurrentActionMap(GAMEPLAY_MAP);
        }
    }
}
