using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestiona el Canvas del reloj digital.
/// Muestra la hora actual y permite subirla/bajarla con botones.
/// </summary>
public class ClockUI : MonoBehaviour
{
    [Header("Canvas")]
    [Tooltip("El Canvas raíz del reloj")]
    public Canvas clockCanvas;

    [Header("Display")]
    [Tooltip("TextMeshPro que muestra la hora HH:MM")]
    public TextMeshProUGUI timeText;

    [Header("Botones")]
    public Button increaseButton; // Adelantar hora
    public Button decreaseButton; // Retrasar hora
    public Button closeButton; // Cerrar el panel (opcional, también se puede cerrar con Escape)

    [Header("Configuración")]
    [Tooltip("Minutos que se avanzan/retroceden por pulsación")]
    public int minutesStep = 15;

    [Tooltip("Hora inicial al abrir el reloj (0–23)")]
    public int startHour = 12;

    [Tooltip("Minutos iniciales al abrir el reloj (0–59)")]
    public int startMinutes = 0;

    [Header("Puzzle — Solución")]
    [Tooltip("Hora correcta para resolver el puzzle (0–23)")]
    public int solutionHour = 7;

    [Tooltip("Minutos correctos para resolver el puzzle (0–59)")]
    public int solutionMinutes = 30;

    [Tooltip("GameObject de la llave que se activará al resolver el puzzle")]
    public GameObject keyObject;

    [Header("Cajón a abrir al resolver")]
    [Tooltip("Cajón que se abrirá automáticamente al poner la hora correcta")]
    public Drawer drawer;
            [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip clockresolved,
      clockchange;

    // Estado interno
    private int currentHour;
    private int currentMinutes;

    // ─── Unity lifecycle ──────────────────────────────────────────────────────

    private void Awake()
    {
        // Suscribir botones
        if (increaseButton != null)
            increaseButton.onClick.AddListener(IncreaseTime);

        if (decreaseButton != null)
            decreaseButton.onClick.AddListener(DecreaseTime);

        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        // Cerrar con ESC / botón cerrar se puede añadir externamente
        Hide(); // Empieza oculto
    }

    private void Update()
    {
        // Cerrar el canvas con Escape
    }

    // ─── API pública ──────────────────────────────────────────────────────────

    /// <summary>Muestra el canvas y resetea la hora al valor inicial.</summary>
    public void Show()
    {
        currentHour = startHour;
        currentMinutes = startMinutes;

        UpdateDisplay();
        clockCanvas.gameObject.SetActive(true);

        // Bloquear cursor del jugador mientras el panel esté abierto
        SetPlayerControl(false);
    }

    /// <summary>Oculta el canvas.</summary>
    public void Hide()
    {
        clockCanvas.gameObject.SetActive(false);
        SetPlayerControl(true);
    }

    /// <summary>Devuelve la hora actual como (horas, minutos).</summary>
    public (int hour, int minutes) GetCurrentTime() => (currentHour, currentMinutes);

    // ─── Lógica de botones ────────────────────────────────────────────────────

    private void IncreaseTime()
    {
        currentMinutes += minutesStep;

        if (currentMinutes >= 60)
        {
            currentMinutes -= 60;
            currentHour = (currentHour + 1) % 24;
        }

        UpdateDisplay();
        OnTimeChanged();
    }

    private void DecreaseTime()
    {
        currentMinutes -= minutesStep;

        if (currentMinutes < 0)
        {
            currentMinutes += 60;
            currentHour = (currentHour - 1 + 24) % 24;
        }

        UpdateDisplay();
        OnTimeChanged();
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private void UpdateDisplay()
    {
        if (timeText != null)
            timeText.text = $"{currentHour:D2}:{currentMinutes:D2}";
    }

    /// <summary>
    /// Llamado cada vez que cambia la hora.
    /// Aquí puedes añadir lógica de puzzle (comprobar solución, efectos, etc.)
    /// </summary>
    private void OnTimeChanged()
    {
        Debug.Log($"[ClockUI] Hora actual: {currentHour:D2}:{currentMinutes:D2}");
                            if (audioSource != null && clockchange != null)
                audioSource.PlayOneShot(clockchange);
        if (currentHour == solutionHour && currentMinutes == solutionMinutes)
        {
            AudioSource.PlayClipAtPoint(clockresolved, Camera.main.transform.position);
            Debug.Log("[ClockUI] ¡Hora correcta! Puzzle resuelto.");
            SolvePuzzle();
        }
    }

    private void SolvePuzzle()
    {
        if (keyObject != null)
            keyObject.SetActive(true);
        else
            Debug.LogWarning("[ClockUI] keyObject no asignado en el Inspector.");

        Hide();
        drawer?.Unlock();
    }

    /// <summary>Activa/desactiva los controles del jugador (movimiento + look).</summary>
    private void SetPlayerControl(bool enabled)
    {
        // Si tienes un PlayerController o CharacterController, desactívalo aquí.
        // Ejemplo genérico — adapta al nombre real de tu script de movimiento:
        //
        // var player = FindFirstObjectByType<PlayerController>();
        // if (player != null) player.enabled = enabled;

        // Bloquear / liberar cursor

        if (!enabled)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
