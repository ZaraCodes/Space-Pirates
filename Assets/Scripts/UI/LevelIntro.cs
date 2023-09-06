using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles the intro to each location with a little loading screen animation and a sound
/// </summary>
public class LevelIntro : MonoBehaviour
{
    #region Fields
    /// <summary>Reference to the music source of the scene</summary>
    [SerializeField] private AudioSource musicSource;
    /// <summary>Reference to the audio source that plays the landing sound</summary>
    [SerializeField] private AudioSource landingSource;

    /// <summary>if true, it plays the intro, if not, it jumps instantly into the action</summary>
    [SerializeField] private bool playIntro;
    /// <summary>The location of this intro</summary>
    [SerializeField] private ELastVisitedLocation location;

    /// <summary>Cache of the initial fade time</summary>
    private float originalFadeTime;
    #endregion

    #region Methods
    /// <summary>
    /// Coroutine that plays the animation and starts the music
    /// </summary>
    /// <returns></returns>
    private IEnumerator Intro()
    {
        ProgressionManager.Instance.LastVisitedLocation = location;
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
    /// <summary>
    /// Starts the intro
    /// </summary>
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
        else
        {
            musicSource.Play();
        }
    }
    #endregion
}
