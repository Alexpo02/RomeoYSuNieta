using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoteCanvas : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;

    [SerializeField]
    private TextMeshProUGUI noteTextComponent;

    private RoomCondition currentRoomCondition;

    private const string DIALOGUE_MAP = "Dialogue";
    private const string GAMEPLAY_MAP = "Player";

    private void Start()
    {
        gameObject.SetActive(false);
    }

    // Ahora recibe el texto y la condición desde quien lo llama
    public void Show(string text, RoomCondition roomCondition)
    {
        currentRoomCondition = roomCondition;

        if (noteTextComponent != null)
            noteTextComponent.text = text;

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

        currentRoomCondition?.Complete();
    }
}
