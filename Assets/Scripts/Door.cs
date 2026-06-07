using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractuable
{
    [Header("Configuración")]
    [SerializeField]
    private float openAngle = 90f;

    [SerializeField]
    private float rotateDuration = 0.5f;

    [Header("Condición de desbloqueo")]
    [Tooltip(
        "Condición que debe cumplirse para poder abrir esta puerta. Dejar vacío si la puerta siempre es accesible."
    )]
    [SerializeField]
    private RoomCondition requiredCondition;

    [Tooltip("Diálogo que aparece al intentar abrir la puerta sin cumplir la condición.")]
    [SerializeField]
    private DialogueData lockedDialogue;

    private bool isOpen;
    private bool isRotating;

    [SerializeField]
    private BoxCollider col;

    private Quaternion closedRotation;
    private Quaternion openedRotation;

    [Header("Audio")]
    [SerializeField]
    private AudioClip doropen;

    private void Awake()
    {
        closedRotation = transform.localRotation;
        openedRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    // ─────────────────────────────────────────────
    // IInteractuable
    // ─────────────────────────────────────────────

    public void Interact()
    {
        if (isRotating)
            return;

        // Si hay condición y no se ha cumplido, muestra el diálogo de bloqueado
        if (requiredCondition != null && !requiredCondition.IsCompleted)
        {
            if (lockedDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(lockedDialogue);
            }
            
            else
                Debug.Log("[Door] La puerta está bloqueada.");
            return;
        }

        if (isOpen)
        {
            StartCoroutine(RotateDoor(closedRotation));
            //col.enabled = false;
        }
        else
                                    if (doropen != null)
                    AudioSource.PlayClipAtPoint(doropen, Camera.main.transform.position);
            StartCoroutine(RotateDoor(openedRotation));
    }

    public void GetInteractionText() { }

    public void HideText() { }

    // ─────────────────────────────────────────────
    // Animación
    // ─────────────────────────────────────────────

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        isRotating = true;

        Quaternion startRotation = transform.localRotation;
        float elapsed = 0f;

        while (elapsed < rotateDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rotateDuration;
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.localRotation = targetRotation;
        isOpen = !isOpen;
        isRotating = false;
    }
}
