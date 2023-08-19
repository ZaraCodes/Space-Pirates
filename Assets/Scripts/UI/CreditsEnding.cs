using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// This handles what happens at the ending of the credits
/// </summary>
public class CreditsEnding : MonoBehaviour
{
    /// <summary>
    /// A unity event that gets invoked after the credits have finished scrolling.
    /// </summary>
    [SerializeField] private UnityEvent onCreditsFinished;
    
    /// <summary>
    /// Reference to the music from captain meow
    /// </summary>
    [SerializeField] private AudioSource meowMusic;

    /// <summary>
    /// Loads the main menu
    /// </summary>
    public void LoadMainMenu()
    {
        StartCoroutine(EndCredits());
    }

    /// <summary>
    /// Coroutine that fades the music and switches the scene
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndCredits()
    {
        var fadeTimeBuffer = GameManager.Instance.PauseMenuHandler.FadeTime;
        GameManager.Instance.PauseMenuHandler.FadeTime = 1.5f;
        StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeIn(null));

        var maxTime = 2f;
        var time = maxTime;
        var volume = meowMusic.volume;

        while (time > 0)
        {
            yield return null;
            time -= Time.deltaTime;
            meowMusic.volume = (time / maxTime) * volume;
        }
        yield return null;
        GameManager.Instance.PauseMenuHandler.FadeTime = fadeTimeBuffer;
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Triggers when the credits have finished
    /// </summary>
    /// <param name="collision">Collision of the other object (unused)</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        onCreditsFinished.Invoke();
    }
}
