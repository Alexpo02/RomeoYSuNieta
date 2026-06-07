using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroScreen : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string nextSceneName = "01Casa";
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
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnVideoFinished;
    }
}

