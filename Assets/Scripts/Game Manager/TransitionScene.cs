using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionScene : MonoBehaviour
{
    public GameObject Canvas;
    private float alpha = 1;
    private GameObject MusicManager;

    void Start()
    {
        Canvas.GetComponent<CanvasGroup>().alpha = 1;
        MusicManager = GameObject.FindGameObjectWithTag("MusicManager");

        StartCoroutine(FadeIn());
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    public void StartFadeOut(string SceneName = "")
    {
        if (SceneName != "") StartCoroutine(FadeOut(true, SceneName));
        else StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        while (alpha > 0)
        {
            alpha -= 0.02f;
            Canvas.GetComponent<CanvasGroup>().alpha = alpha;
            yield return new WaitForSeconds(0.007f);
        }
        Canvas.SetActive(false);
    }

    private IEnumerator FadeOut(bool ChangeScene = false, string SceneName = "")
    {
        Canvas.SetActive(true);
        while (alpha < 1)
        {
            alpha += 0.02f;
            Canvas.GetComponent<CanvasGroup>().alpha = alpha;
            yield return new WaitForSeconds(0.007f);
        }
        if (ChangeScene)
        {
            if (SceneName == "Quit") Application.Quit();
            else SceneManager.LoadScene(SceneName);
        }
    }

    public void StartMusicFadeout(bool DestroyMusic = false)
    {
        StartCoroutine(MusicFadeout(DestroyMusic));
    }

    private IEnumerator MusicFadeout(bool DestroyMusic = false)
    {
        float musvol = MusicManager.GetComponent<AudioSource>().volume;
        float musvoltime = 1f;

        while (musvoltime > 0)
        {
            musvoltime -= 0.02f;
            if (musvoltime < 0) musvoltime = 0;
            MusicManager.GetComponent<AudioSource>().volume = musvol * musvoltime;
            yield return new WaitForSeconds(0.007f);
        }

        if (DestroyMusic) Destroy(GameObject.FindGameObjectWithTag("MusicKeepPlay"));
    }
}
