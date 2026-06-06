using System;
using UnityEngine;

public enum SpeakerPortrait
{
    None,
    Character1,
    Character2,
    Character3,
}

[Serializable]
public struct DialogueLine
{
    [Tooltip("Nombre que aparecerá en la UI (ej: 'Protagonista', 'Guardia')")]
    public string speakerName;

    [TextArea(2, 5)]
    public string text;

    [Tooltip("Retrato del personaje que aparecerá en el canvas")]
    public SpeakerPortrait portrait;
}
