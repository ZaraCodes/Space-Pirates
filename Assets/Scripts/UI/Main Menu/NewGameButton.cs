using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Starts a new game or opens the scene chooser window
/// </summary>
public class NewGameButton : MonoBehaviour
{
    /// <summary>Reference to the scene chooser</summary>
    [SerializeField] private GameObject sceneChooser;
    /// <summary>Reference to the main menu</summary>
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private AudioSource buttonAudio;

    [SerializeField] private Image fadeImage;

    public UnityEvent OnFadeFinished;
    /// <summary>
    /// Opens the scene chooser window
    /// </summary>
    public void OpenSceneChooser()
    {
        sceneChooser.SetActive(true);
        mainMenu.SetActive(false);
    }

    /// <summary>
    /// Loads the Intro scene
    /// </summary>
    public void StartNewGame()
    {
        SceneManager.LoadScene(5);
    }

    public void StartButtonPressed(float fadeTime)
    {
        buttonAudio.Play();
        StartFadeIn(fadeTime);
    }

    public void StartFadeIn(float fadeTime)
    {
        StartCoroutine(FadeOut(fadeTime));
    }

    private IEnumerator FadeOut(float fadeTime)
    {
        fadeImage.gameObject.SetActive(true);
        float timer = 0;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, timer / fadeTime);
            Debug.Log(timer/fadeTime);
            yield return null;
        }
        OnFadeFinished.Invoke();
    }

    /// <summary>
    /// Resets the menu and a game manager setting
    /// </summary>
    private void Start()
    {
        GameManager.Instance.IsSceneIntroPlaying = false;
        GetComponent<Button>().Select();
    }
}
