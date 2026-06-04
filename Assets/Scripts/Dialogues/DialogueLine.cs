using System;
using UnityEngine;

[Serializable]
public struct DialogueLine
{
    [Tooltip("Nombre que aparecerá en la UI (ej: 'Protagonista', 'Guardia')")]
    public string speakerName;

    [TextArea(2, 5)]
    public string text;
}
