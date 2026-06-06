using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel negro que cubre la pantalla al inicio y hace fade out
/// cuando se le llama desde el onDialogueEnd del DialogueTrigger de introducción.
/// </summary>
public class IntroOverlay : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField]
    private CanvasGroup canvasGroup;

    [Header("Fade")]
    [SerializeField]
    private float fadeDuration = 1.5f;

    private void Awake()
    {
        // Aseguramos que empieza completamente opaco
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Llama a este método desde el UnityEvent onDialogueEnd del DialogueTrigger.
    /// </summary>
    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false); // Limpieza final
    }
}
