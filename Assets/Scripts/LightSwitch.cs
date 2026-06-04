using UnityEngine;

public class LightSwitch : MonoBehaviour, IInteractuable
{
    [SerializeField]
    private Light targetLight;

    private bool isOn = true;

    private void Start()
    {
        if (targetLight != null)
        {
            isOn = targetLight.enabled;
        }
    }

    public void Interact()
    {
        if (targetLight == null)
            return;

        isOn = !isOn;
        targetLight.enabled = isOn;
    }

    public void GetInteractionText()
    {
        Debug.Log("Pulsa E para encender/apagar la luz");
        // Aquí mostrarías tu UI
    }

    public void HideText()
    {
        // Ocultar UI
    }
}
