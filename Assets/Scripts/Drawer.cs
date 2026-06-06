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

    [Header("Bloqueo")]
    [Tooltip(
        "Si true, el jugador no puede abrir el cajón manualmente hasta que se llame a Unlock()"
    )]
    [SerializeField]
    private bool lockedUntilSolved = false;

    [Header("Contenido")]
    [Tooltip(
        "Hijos del cajón que se activarán al abrirlo (arrástralos aquí o déjalos vacío para cogerlos automáticamente)"
    )]
    [SerializeField]
    private GameObject[] contents;

    private Collider[] drawerColliders;
    private bool isOpen;
    private bool isMoving;
    private bool isUnlocked;

    private void Awake()
    {
        // Cachear todos los colliders del cajón (solo los del propio GameObject, no los hijos)
        drawerColliders = GetComponents<Collider>();
    }

    private void Start()
    {
        transform.localPosition = closedPosition;
        isUnlocked = !lockedUntilSolved;

        // Si contents no se rellenó en el Inspector, recoge todos los hijos inactivos
        if (contents == null || contents.Length == 0)
            CollectInactiveChildren();
    }

    /// <summary>Llamado por el puzzle para desbloquear Y abrir el cajón automáticamente.</summary>
    public void Unlock()
    {
        isUnlocked = true;
        if (!isOpen && !isMoving)
            StartCoroutine(MoveDrawer(openedPosition));
    }

    // ─── IInteractuable ───────────────────────────────────────────────────────

    public void Interact()
    {
        if (!isUnlocked || isMoving || isOpen)
            return; // una vez abierto no se cierra
        StartCoroutine(MoveDrawer(openedPosition));
    }

    public void GetInteractionText() { }

    public void HideText() { }

    // ─── Movimiento ───────────────────────────────────────────────────────────

    private IEnumerator MoveDrawer(Vector3 targetPosition)
    {
        isMoving = true;
        Vector3 startPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(
                startPosition,
                targetPosition,
                elapsed / moveDuration
            );
            yield return null;
        }

        transform.localPosition = targetPosition;
        isOpen = true;
        isMoving = false;

        OnDrawerOpened();
    }

    private void OnDrawerOpened()
    {
        // 1. Desactivar colliders del cajón para que no bloqueen la interacción con el contenido
        foreach (var col in drawerColliders)
            col.enabled = false;

        // 2. Activar el contenido
        foreach (var item in contents)
        {
            if (item != null)
                item.SetActive(true);
        }
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    /// <summary>Recoge automáticamente los hijos que están desactivados en Start.</summary>
    private void CollectInactiveChildren()
    {
        var list = new System.Collections.Generic.List<GameObject>();
        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf)
                list.Add(child.gameObject);
        }
        contents = list.ToArray();
    }
}
