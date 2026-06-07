using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField]
    private CanvasGroup fadeCanvasGroup;

    [SerializeField]
    private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Empezamos en negro si es necesario, o transparente
        fadeCanvasGroup.alpha = 0;
        fadeCanvasGroup.gameObject.SetActive(false);
    }

    public void TransitionToScene(string sceneName, string spawnID)
    {
        StartCoroutine(FadeRoutine(sceneName, spawnID));
    }

    private IEnumerator FadeRoutine(string sceneName, string spawnID)
    {
        //Player.Instance.SetMovementBlocked(true);
        // 1. FADE OUT (Hacia negro)
        fadeCanvasGroup.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(1));

        // Opcional: Una breve pausa totalmente en negro antes de cargar
        yield return new WaitForSecondsRealtime(0.2f);

        // 2. CONFIGURAR SPAWN Y CARGAR ESCENA
        /*if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.SetTargetSpawn(spawnID);
        }*/

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        // Mientras carga, nos aseguramos de que el alpha se mantenga en 1
        while (!op.isDone)
        {
            fadeCanvasGroup.alpha = 1;
            yield return null;
        }

        // Esperamos un par de frames o un tiempo corto para que el
        // Player y la cámara se estabilicen en la nueva escena.
        yield return new WaitForSecondsRealtime(0.7f);

        // 3. FADE IN (Hacia transparente)
        yield return StartCoroutine(Fade(0));
        fadeCanvasGroup.gameObject.SetActive(false);
    }

    private void OnOpaque() { }

    private void OnTransparent()
    {
        //Player.Instance.SetMovementBlocked(false);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        // Verificación de seguridad
        if (fadeCanvasGroup == null)
        {
            Debug.LogError("¡No hay CanvasGroup asignado en SceneTransitionManager!");
            yield break;
        }

        float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / fadeDuration;

        // Si la duración es 0, hacemos el cambio instantáneo
        if (fadeDuration <= 0)
        {
            fadeCanvasGroup.alpha = targetAlpha;
            yield break;
        }

        while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
        {
            // Volvemos a comprobar por si el objeto se destruyó en medio de la corrutina
            if (fadeCanvasGroup == null)
                yield break;

            fadeCanvasGroup.alpha = Mathf.MoveTowards(
                fadeCanvasGroup.alpha,
                targetAlpha,
                speed * Time.deltaTime
            );
            yield return null;
        }
        fadeCanvasGroup.alpha = targetAlpha;
        if (targetAlpha == 1)
        {
            OnOpaque();
        }
        else
        {
            OnTransparent();
        }
    }
}
