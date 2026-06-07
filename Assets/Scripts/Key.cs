using UnityEngine;

public class Key : MonoBehaviour, IInteractuable
{
    [Tooltip("Debe coincidir exactamente con el keyId de la Cage que abre")]
    [SerializeField]
    private string keyId;

    private Collider myCollider;
        
    [Header("Audio")]
    [SerializeField]
    private AudioClip takekey;
    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

    // Cuando la llave se activa en runtime (ej: al resolver el puzzle),
    // fuerza a Unity a redetectar colisiones desactivando y reactivando el collider.
    private void OnEnable()
    {
        if (myCollider != null)
        {
            myCollider.enabled = false;
            myCollider.enabled = true;
        }
    }

    public void GetInteractionText() { }

    public void HideText() { }

    public void Interact()
    {
        if (takekey != null)
            AudioSource.PlayClipAtPoint(takekey, Camera.main.transform.position);
        KeyInventory.Instance.AddKey(keyId);
        Destroy(gameObject);
    }
}
