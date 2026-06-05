using UnityEngine;

public class Key : MonoBehaviour, IInteractuable
{
    [Tooltip("Debe coincidir exactamente con el keyId de la Cage que abre")]
    [SerializeField]
    private string keyId;

    public void GetInteractionText() { }

    public void HideText() { }

    public void Interact()
    {
        KeyInventory.Instance.AddKey(keyId);
        Destroy(gameObject);
    }
}
