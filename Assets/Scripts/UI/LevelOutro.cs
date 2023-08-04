using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelOutro : MonoBehaviour
{
    #region Fields
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource liftoffSource;

    [SerializeField] private ELastVisitedLocation location;

    [SerializeField] private float musicFadeTime;
    #endregion

    #region Methods
    public void StartOutro()
    {
        GameManager.Instance.IsSceneIntroPlaying = true;
        liftoffSource.Play();
        StartCoroutine(Outro());
    }

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
