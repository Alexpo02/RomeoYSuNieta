using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Canvas de nota que se comporta como un diálogo:
/// bloquea al jugador y se cierra con E.
/// Al cerrarse, marca la RoomCondition de su sala como completada.
/// Llama a Show() desde el onDialogueEnd del DialogueTrigger del pájaro.
/// </summary>
public class NoteCanvas : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;

    [Header("Condición de sala")]
    [Tooltip("RoomCondition de esta sala. Se marca como completada al cerrar la nota.")]
    [SerializeField]
    private RoomCondition roomCondition;

    private const string DIALOGUE_MAP = "Dialogue";
    private const string GAMEPLAY_MAP = "Player";

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        playerInput.SwitchCurrentActionMap(DIALOGUE_MAP);
    }

    public void OnClose(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
            return;
        if (!gameObject.activeSelf)
            return;

        gameObject.SetActive(false);
        playerInput.SwitchCurrentActionMap(GAMEPLAY_MAP);

        // Marca la condición de la sala como cumplida
        roomCondition?.Complete();
    }
}
