using UnityEngine;

public class InteractionTextDisplay : MonoBehaviour
{
    public void ShowText(string text)
    {
        Debug.Log($"[InteractionTextDisplay] Mostrar texto: {text}");
        // Aquí se implementaría la lógica para mostrar el texto en pantalla
    }

    public void HideText()
    {
        Debug.Log("[InteractionTextDisplay] Ocultar texto");
        // Aquí se implementaría la lógica para ocultar el texto en pantalla
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
