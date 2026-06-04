using System.Collections;
using UnityEngine;

public class Drawer : MonoBehaviour, IInteractuable
{
    [Header("Posiciones")]
    [SerializeField]
    private Vector3 closedPosition;

    [SerializeField]
    private Vector3 openedPosition;

    [Header("Animación")]
    [SerializeField]
    private float moveDuration = 0.5f;

    private bool isOpen;
    private bool isMoving;

    private void Start()
    {
        transform.localPosition = closedPosition;
    }

    public void Interact()
    {
        if (isMoving)
            return;

        if (isOpen)
            StartCoroutine(MoveDrawer(closedPosition));
        else
            StartCoroutine(MoveDrawer(openedPosition));
    }

    private IEnumerator MoveDrawer(Vector3 targetPosition)
    {
        isMoving = true;

        Vector3 startPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / moveDuration;

            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        transform.localPosition = targetPosition;

        isOpen = !isOpen;
        isMoving = false;
    }

    public string GetInteractionText()
    {
        return isOpen ? "Cerrar cajón" : "Abrir cajón";
    }

    public void HideText()
    {
        // Ocultar UI aquí
    }

    void IInteractuable.GetInteractionText()
    {
        //throw new System.NotImplementedException();
    }
}
