using System.Collections;
using UnityEngine;

/// <summary>
/// Añade un flash blanco al objeto cuando el jugador entra en su zona de interacción.
/// Usa MaterialPropertyBlock para no crear instancias de material (sin impacto en batching).
///
/// SETUP:
///   1. Añade este componente al mismo GameObject que tiene IInteractuable.
///   2. El shader del material debe exponer _BaseColor o _Color (URP/Built-in).
///      Si usas URP Lit, funciona de serie.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class InteractableHighlight : MonoBehaviour
{
    [Header("Flash")]
    [Tooltip("Color del destello (blanco por defecto)")]
    public Color flashColor = Color.white;

    [Tooltip("Duración total del flash (ida + vuelta) en segundos")]
    public float flashDuration = 0.3f;

    [Tooltip("Intensidad máxima del flash (0 = sin efecto, 1 = color puro)")]
    [Range(0f, 1f)]
    public float flashIntensity = 0.6f;

    // ─── Privados ─────────────────────────────────────────────────────────────

    private Renderer[] _renderers;
    private MaterialPropertyBlock _mpb;
    private Color _originalColor;
    private Coroutine _flashCoroutine;

    // Nombre de la propiedad de color — URP usa "_BaseColor", Built-in usa "_Color"
    private static readonly int ColorPropURP = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorPropBuiltIn = Shader.PropertyToID("_Color");

    // ─── Unity lifecycle ──────────────────────────────────────────────────────

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        if (_renderers.Length == 0)
        {
            enabled = false;
            return;
        }

        // DEBUG — mira la consola de Unity
        foreach (var r in _renderers)
        {
            var mat = r.sharedMaterial;
            Debug.Log(
                $"[Highlight] {r.gameObject.name} | Shader: {mat.shader.name} | "
                    + $"Tiene _BaseColor: {mat.HasProperty(ColorPropURP)} | "
                    + $"Tiene _Color: {mat.HasProperty(ColorPropBuiltIn)}"
            );
        }

        var firstMat = _renderers[0].sharedMaterial;
        _originalColor = firstMat.HasProperty(ColorPropURP)
            ? firstMat.GetColor(ColorPropURP)
            : firstMat.GetColor(ColorPropBuiltIn);

        _mpb = new MaterialPropertyBlock();
    }

    // ─── API pública ──────────────────────────────────────────────────────────

    /// <summary>Lanza el flash. Llámalo desde GetInteractionText().</summary>
    public void TriggerFlash()
    {
        if (_flashCoroutine != null)
            StopCoroutine(_flashCoroutine);

        _flashCoroutine = StartCoroutine(FlashRoutine());
    }

    /// <summary>Restaura el color original inmediatamente. Llámalo desde HideText().</summary>
    public void ResetColor()
    {
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
            _flashCoroutine = null;
        }

        ApplyColor(_originalColor);
    }

    // ─── Coroutine ────────────────────────────────────────────────────────────

    private IEnumerator FlashRoutine()
    {
        float half = flashDuration * 0.5f;
        float t = 0f;

        // Ida: original → flash
        while (t < half)
        {
            t += Time.deltaTime;
            ApplyColor(Color.Lerp(_originalColor, flashColor, (t / half) * flashIntensity));
            yield return null;
        }

        // Vuelta: flash → original
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            ApplyColor(Color.Lerp(flashColor, _originalColor, t / half));
            yield return null;
        }

        ApplyColor(_originalColor);
        _flashCoroutine = null;
    }

    private void ApplyColor(Color color)
    {
        foreach (var r in _renderers)
        {
            r.GetPropertyBlock(_mpb);
            var mat = r.sharedMaterial;
            if (mat.HasProperty(ColorPropURP))
                _mpb.SetColor(ColorPropURP, color);
            else
                _mpb.SetColor(ColorPropBuiltIn, color);
            r.SetPropertyBlock(_mpb);
        }
    }
}
