using UnityEngine;

/// <summary>
/// Se pone en cada objeto (los 3). Cada uno tiene su propio texto
/// y apunta al canvas compartido. Llámase Show() desde onDialogueEnd.
/// </summary>
public class NoteTrigger : MonoBehaviour
{
    [Header("Canvas compartido (el mismo en los 3 objetos)")]
    [SerializeField]
    private NoteCanvas noteCanvas;

    [Header("Texto exclusivo de este objeto")]
    [SerializeField, TextArea(3, 10)]
    private string noteText;

    [Header("Condición de sala de este objeto")]
    [SerializeField]
    private RoomCondition roomCondition;

    // Este método se arrastra en el onDialogueEnd del DialogueTrigger
    public void Show()
    {
        noteCanvas.Show(noteText, roomCondition);
    }
}
