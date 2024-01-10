using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Video;
using System;

public class W94_UIManager : MonoBehaviour
{
    [SerializeField] private float remainingTime;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Image blackScreen;
    [SerializeField] private Image frameLower;
    [SerializeField] private Image frameOuter;
    private float time;
    private bool lockFlag = true;

    [Header("Intro variables")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoOnCanvas;
    [SerializeField] private GameObject skipButton;
    private int introWatchedBefore;

    private void Start()
    {
        time = remainingTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (W94_GameManager.instance.state == W94_GameManager.GameState.playing)
            Timer();
    }

    private void Timer()
    {
        time -= Time.deltaTime;

        timeText.text = time.ToString("00");

        if (time <= 0 && lockFlag)
        {
            lockFlag = false;
            StartCoroutine(TimesUp());
        }
    }
    IEnumerator TimesUp()
    {
        W94_GameManager.instance.state = W94_GameManager.GameState.idle;

        W94_AudioManager.instance.Stop("Background");
        W94_AudioManager.instance.Play("TimesUp");

        blackScreen.gameObject.SetActive(true);
        blackScreen.DOFade(1, 4);
        blackScreen.transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(1, 4);
        yield return new WaitForSeconds(4f);

        W94_GameManager.instance.Finish();
    }

    public void DisableBlackScreen()
    {
        blackScreen.DOFade(0, 0.1f);
        blackScreen.transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(0, 0.1f);
        blackScreen.gameObject.SetActive(false);
    }

    public void ResetTime()
    {
        time = remainingTime;
    }

    public void ArrangeFrames(Transform parent)
    {
        frameLower.transform.SetParent(parent);
        frameOuter.transform.SetParent(parent);

        frameLower.transform.SetAsLastSibling();
        frameOuter.transform.SetAsLastSibling();
    }

    #region Intro
    public void StartIntro()
    {
        videoPlayer.Play();
        Invoke("EndReached", (float)videoPlayer.clip.length);
        Invoke("FadeOut", (float)videoPlayer.clip.length - 3f);
        introWatchedBefore = PlayerPrefs.GetInt("W94_introWatchedBefore", 0);
        if (introWatchedBefore == 1)
            skipButton.SetActive(true);
    }

    private void EndReached()
    {
        //close the gameobject and stop the video
        videoOnCanvas.SetActive(false);
        PlayerPrefs.SetInt("W94_introWatchedBefore", 1);
        skipButton.SetActive(false);
        videoPlayer.Stop();
        W94_GameManager.instance.state = W94_GameManager.GameState.idle;
        W94_GameManager.instance.StartGame();
    }

    public void SkipIntro()
    {
        CancelInvoke();
        videoPlayer.Stop();
        skipButton.SetActive(false);
        videoOnCanvas.SetActive(false);
        W94_GameManager.instance.state = W94_GameManager.GameState.idle;
        W94_GameManager.instance.StartGame();
    }

    public void PlayPauseVideo()
    {
        videoPlayer.Play();
        videoPlayer.Pause();
    }

    public void FadeOut()
    {
        StartCoroutine(LerpFunction(0f, 3f));
    }

    IEnumerator LerpFunction(float endValue, float duration)
    {
        float time = 0;
        float startValue = videoPlayer.GetDirectAudioVolume(0);
        while (time < duration)
        {
            videoPlayer.SetDirectAudioVolume(0, Mathf.Lerp(startValue, endValue, time / duration));
            time += Time.deltaTime;
            yield return null;
        }
        videoPlayer.SetDirectAudioVolume(0, endValue);
    }
    #endregion
}
