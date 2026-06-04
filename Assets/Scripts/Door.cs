using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractuable
{
    [Header("Configuración")]
    [SerializeField]
    private float openAngle = 90f;

    [SerializeField]
    private float rotateDuration = 0.5f;

    private bool isOpen;
    private bool isRotating;

    private Quaternion closedRotation;
    private Quaternion openedRotation;

    private void Awake()
    {
        closedRotation = transform.localRotation;
        openedRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    public void Interact()
    {
        if (isRotating)
            return;

        if (isOpen)
            StartCoroutine(RotateDoor(closedRotation));
        else
            StartCoroutine(RotateDoor(openedRotation));
    }

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

    public string GetInteractionText()
    {
        return isOpen ? "Cerrar puerta" : "Abrir puerta";
    }

    public void HideText()
    {
        // Ocultar UI
    }

    void IInteractuable.GetInteractionText()
    {
        //throw new System.NotImplementedException();
    }
}
