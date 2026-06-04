using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Tooltip("Lista de líneas de este diálogo. Pueden ser de distintos personajes intercalados.")]
    public DialogueLine[] lines;
}
