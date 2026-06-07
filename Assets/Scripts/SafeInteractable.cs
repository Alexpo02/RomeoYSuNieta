using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Coloca este script en el GameObject de la caja fuerte.
/// Requiere un Collider con Is Trigger activado.
///
/// En el Inspector asigna:
///   - safeCanvas        → el GameObject del Canvas
///   - displayText       → el TextMeshPro de la "pantalla" (muestra ● ● _ _ _)
///   - buttons[0..8]     → los 9 botones (en orden, botón 1 primero)
///   - correctCode       → la combinación correcta, ej. "47391"
/// </summary>
[RequireComponent(typeof(Collider))]
public class SafeInteractable : MonoBehaviour, IInteractuable
{
    [Header("Combinación")]
    public string correctCode = "47391";

    [Header("UI")]
    public GameObject safeCanvas;
    public TextMeshProUGUI displayText; // Pantalla: ● ● ● _ _
    public Button[] buttons = new Button[9];
    public Button closeButton;

    [Header("Recompensa")]
    [Tooltip("GameObject de la llave que aparece al abrir la caja")]
    public GameObject keyObject;

    [Header("Pista de interacción")]
    public TextMeshProUGUI hintText;
    public string hintMessage = "Pulsa E para abrir la caja fuerte";

    [Header("Puerta de la caja")]
    [Tooltip("Script Door de la puerta de la caja fuerte")]
    [SerializeField]
    private Door safeDoor;

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip safecagechange,savecageresolved,savecageerror;


    // ── Estado ──────────────────────────────────────────────────────
    private string input = "";
    private bool isUnlocked = false;
    private bool busy = false; // bloquea botones durante animación

    // ── IInteractuable ───────────────────────────────────────────────

    public void Interact()
    {
        if (isUnlocked)
            return;
        OpenCanvas();
    }

    public void GetInteractionText()
    {
        if (isUnlocked)
            return;
        if (hintText != null)
        {
            hintText.text = hintMessage;
            hintText.gameObject.SetActive(true);
        }
    }

    public void HideText()
    {
        if (hintText != null)
            hintText.gameObject.SetActive(false);
    }

    // ── Canvas ───────────────────────────────────────────────────────

    void OpenCanvas()
    {
        input = "";
        busy = false;
        RefreshDisplay();

        safeCanvas.SetActive(true);
        SetPlayerLocked(true);
    }

    public void CloseCanvas()
    {
        safeCanvas.SetActive(false);
        SetPlayerLocked(false);
    }

    // ── Start: conecta los botones ───────────────────────────────────
    void Hide()
    {
        CloseCanvas();
    }

    void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);
    }

    void Start()
    {
        safeCanvas.SetActive(false);

        for (int i = 0; i < buttons.Length; i++)
        {
            int digit = i + 1; // botón 0 → dígito "1", etc.
            buttons[i].onClick.AddListener(() => OnDigit(digit.ToString()));
        }

        RefreshDisplay();
    }

    // ── Lógica ───────────────────────────────────────────────────────

    void OnDigit(string d)
    {
        if (busy)
            return;
        if (input.Length >= correctCode.Length)
            return;
        if (safecagechange != null)
            AudioSource.PlayClipAtPoint(safecagechange, Camera.main.transform.position);
        
        input += d;
        RefreshDisplay();

        if (input.Length == correctCode.Length)
            StartCoroutine(Validate());
    }

    IEnumerator Validate()
    {
        busy = true;
        yield return new WaitForSeconds(0.3f);

        if (input == correctCode)
        {
            // ── CORRECTO ─────────────────────────────────────────────
            if (savecageresolved != null)
                    AudioSource.PlayClipAtPoint(savecageresolved, Camera.main.transform.position);
            displayText.text = "✓";
            yield return new WaitForSeconds(0.6f);

            isUnlocked = true;
            CloseCanvas();
            OnSafeOpen();
        }
        else
        {
            // ── ERROR ────────────────────────────────────────────────
            if (savecageerror != null)
                AudioSource.PlayClipAtPoint(savecageerror, Camera.main.transform.position);
            displayText.text = "ERROR";
            yield return StartCoroutine(ShakeDisplay());
            yield return new WaitForSeconds(0.4f);

            input = "";
            RefreshDisplay();
            busy = false;
        }
    }

    // ── Pantalla ─────────────────────────────────────────────────────

    void RefreshDisplay()
    {
        if (displayText == null)
            return;

        string s = "";
        for (int i = 0; i < correctCode.Length; i++)
            s += (i < input.Length) ? "● " : "_ ";

        displayText.text = s.TrimEnd();
    }

    // ── Apertura de la caja ──────────────────────────────────────────

    void OnSafeOpen()
    {
        if (keyObject != null)
            keyObject.SetActive(true);

        safeDoor?.Interact();

        // Aquí engancha tu Animator cuando tengas el modelo:
        // GetComponent<Animator>()?.SetTrigger("Open");
        Debug.Log("¡Caja fuerte abierta!");
    }

    // ── Helpers ──────────────────────────────────────────────────────

    IEnumerator ShakeDisplay()
    {
        RectTransform rt = displayText.rectTransform;
        Vector3 origin = rt.localPosition;
        float t = 0f;
        while (t < 0.35f)
        {
            rt.localPosition = origin + new Vector3(Random.Range(-6f, 6f), 0, 0);
            t += Time.deltaTime;
            yield return null;
        }
        rt.localPosition = origin;
    }

    void SetPlayerLocked(bool locked)
    {
        var player = FindFirstObjectByType<PlayerInteractor>();
        if (player != null)
            player.enabled = !locked;

        Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = locked;
    }
}
