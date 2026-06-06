using UnityEngine;

public class LightSwitch : MonoBehaviour, IInteractuable
{
    [SerializeField]
    private Light[] targetLights;

    [SerializeField]
    private GameObject worldSpaceCanvas;

    private bool isOn = true;

    private void Start()
    {
        if (targetLights != null && targetLights.Length > 0 && targetLights[0] != null)
            isOn = targetLights[0].enabled;

        if (worldSpaceCanvas != null)
            worldSpaceCanvas.SetActive(!isOn);
    }

    public void Interact()
    {
        if (targetLights == null || targetLights.Length == 0)
            return;

        isOn = !isOn;

        foreach (Light light in targetLights)
        {
            if (light != null)
                light.enabled = isOn;
        }

        if (worldSpaceCanvas != null)
            worldSpaceCanvas.SetActive(!isOn);
    }

    public void GetInteractionText()
    {
        Debug.Log("Pulsa E para encender/apagar la luz");
    }

    public void HideText() { }
}
