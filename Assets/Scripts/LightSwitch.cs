using UnityEngine;

public class LightSwitch : MonoBehaviour, IInteractuable
{
    [SerializeField]
    private Light targetLight;

    [SerializeField]
    private GameObject worldSpaceCanvas;

    private bool isOn = true;

    private void Start()
    {
        if (targetLight != null)
            isOn = targetLight.enabled;

        // El canvas solo aparece cuando la luz está apagada
        if (worldSpaceCanvas != null)
            worldSpaceCanvas.SetActive(!isOn);
    }

    public void Interact()
    {
        if (targetLight == null)
            return;

        isOn = !isOn;
        targetLight.enabled = isOn;

        if (worldSpaceCanvas != null)
            worldSpaceCanvas.SetActive(!isOn);
    }

    public void GetInteractionText()
    {
        Debug.Log("Pulsa E para encender/apagar la luz");
    }

    public void HideText() { }
}
