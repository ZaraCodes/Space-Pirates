using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelIntro : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource landingSource;

    [SerializeField] private bool playIntro;

    private float originalFadeTime;

    private IEnumerator Intro()
    {
        yield return new WaitForSeconds(3f);
        musicSource.Play();

        UnityEvent onFadeOutFinished = new();
        onFadeOutFinished.AddListener(() => GameManager.Instance.PauseMenuHandler.FadeTime = originalFadeTime);

        StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeOut(onFadeOutFinished));

        yield return new WaitForSeconds(.5f);
        GameManager.Instance.Nova.Fading = false;
    }

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

            GameManager.Instance.Nova.Fading = true;

            StartCoroutine(Intro());
        }
    }
}
