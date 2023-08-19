using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Handles the outro in each location with a little loading screen animation and a sound
/// </summary>
public class LevelOutro : MonoBehaviour
{
    #region Fields
    /// <summary>Reference to the music source of the scene</summary>
    [SerializeField] private AudioSource musicSource;
    /// <summary>Reference to the audio source that plays the landing sound</summary>
    [SerializeField] private AudioSource liftoffSource;

    /// <summary>The location this outro belongs to</summary>
    [SerializeField] private ELastVisitedLocation location;
    /// <summary>The time it takes to fade the music</summary>
    [SerializeField] private float musicFadeTime;
    #endregion

    #region Methods
    /// <summary>
    /// Starts the outro
    /// </summary>
    public void StartOutro()
    {
        GameManager.Instance.IsSceneIntroPlaying = true;
        liftoffSource.Play();
        StartCoroutine(Outro());
    }

    /// <summary>
    /// Coroutine that plays the outro and loads the space scene
    /// </summary>
    /// <returns></returns>
    private IEnumerator Outro()
    {
        yield return new WaitForSeconds(1f);

        var maxMusicVolume = musicSource.volume;
        GameManager.Instance.PauseMenuHandler.FadeTime = musicFadeTime;
        
        StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeIn(null));
        while (musicSource.volume > 0)
        {
            musicSource.volume -= maxMusicVolume / musicFadeTime * Time.deltaTime;
            yield return null;
        }
        GameManager.Instance.PauseMenuHandler.LoadingScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(8f);
        GameManager.Instance.IsSceneIntroPlaying = false;
        ProgressionManager.Instance.LastVisitedLocation = location;
        SceneManager.LoadScene(1);
    }
    #endregion
}
