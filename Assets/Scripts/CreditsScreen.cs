using UnityEngine;
using UnityEngine.Video;

public class CreditsScreen : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject panelNegro;

    private bool videoStarted = false;

    void Start()
    {
        panelNegro.SetActive(true);
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Prepare();
    }

    void Update()
    {
        if (!videoStarted && videoPlayer.isPrepared)
        {
            panelNegro.SetActive(false);
            videoPlayer.Play();
            videoStarted = true;
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Application.Quit();

        // Solo para probar en el editor de Unity:
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnVideoFinished;
    }
}
