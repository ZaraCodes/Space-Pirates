using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CreditsStarter : MonoBehaviour
{
    /// <summary>
    /// Reference to the credits object
    /// </summary>
    [SerializeField] private GameObject credits;

    /// <summary>Reference to the scrolling background</summary>
    [SerializeField] private GameObject scrollBackground;

    /// <summary>
    /// Starts the credits
    /// </summary>
    public void BeginCredits()
    {
        UnityEvent onFadeInFinished = new();
        onFadeInFinished.AddListener(() =>
        {
            GameManager.Instance.Nova.gameObject.SetActive(false);
            Camera.main.GetComponent<AudioListener>().enabled = true;
            scrollBackground.SetActive(true);

            UnityEvent onFadeOutFinished = new();
            onFadeOutFinished.AddListener(() =>
            {
                credits.SetActive(true);
            });
            StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeOut(onFadeOutFinished));
        });
        StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeIn(onFadeInFinished));
    }
}
