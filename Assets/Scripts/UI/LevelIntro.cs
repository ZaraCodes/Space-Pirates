using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelIntro : MonoBehaviour
{
    #region Fields
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource landingSource;

    [SerializeField] private bool playIntro;

    private float originalFadeTime;
    #endregion

    #region Methods
    private IEnumerator Intro()
    {
        GameManager.Instance.PauseMenuHandler.LoadingScreen.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);
        musicSource.Play();

        UnityEvent onFadeOutFinished = new();
        onFadeOutFinished.AddListener(() => GameManager.Instance.PauseMenuHandler.FadeTime = originalFadeTime);

        GameManager.Instance.IsSceneIntroPlaying = false;
        GameManager.Instance.PauseMenuHandler.LoadingScreen.gameObject.SetActive(false);
        StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeOut(onFadeOutFinished));

        yield return new WaitForSeconds(.5f);
        GameManager.Instance.Nova.Fading = false;
    }
    #endregion

    #region Unity Stuff
    private void Start()
    {
        if (playIntro)
        {
            musicSource.Stop();
            landingSource.Play();
            
            originalFadeTime = GameManager.Instance.PauseMenuHandler.FadeTime;
            GameManager.Instance.PauseMenuHandler.FadeTime = 2f;

            GameManager.Instance.PauseMenuHandler.BlackFade.gameObject.SetActive(true);
            GameManager.Instance.PauseMenuHandler.BlackFade.color = Color.black;

            GameManager.Instance.IsSceneIntroPlaying = true;

            GameManager.Instance.Nova.Fading = true;

            StartCoroutine(Intro());
        }
    }
    #endregion
}
