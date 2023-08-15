using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntroCutscene : MonoBehaviour
{
    #region Fields
    /// <summary>The velocity the ship starts with in the intro cutscene</summary>
    [SerializeField] private Vector2 startingVelocity;

    /// <summary>The cutscene's space ship's rigidbody</summary>
    [SerializeField] private Rigidbody2D shipRigidbody;

    /// <summary>Reference to the first dialog</summary>
    [SerializeField] private TextboxTrigger preMeowDialog;

    /// <summary>Reference to the audio source that plays the first track</summary>
    [SerializeField] private AudioSource preMeowMusic;

    /// <summary>Reference to the audio source that plays the theme for captain meow</summary>
    [SerializeField] private AudioSource captainMeowMusic;
    #endregion

    #region Methods
    /// <summary>Starts the ship movement before the dialog</summary>
    private void StartShipMovement()
    {
        shipRigidbody.velocity = startingVelocity;
    }

    /// <summary>Starts the intro cutscene</summary>
    private void BeginCutscene()
    {
        UnityEvent onFadeInFinished = new();
        onFadeInFinished.AddListener(() =>
        {
            preMeowDialog.LoadDialog();
            shipRigidbody.bodyType = RigidbodyType2D.Static;
        });
        StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeIn(onFadeInFinished));
    }


    public void StartMeowSection()
    {
        StartCoroutine(CrossFadeMusic());
    }
    
    private IEnumerator CrossFadeMusic()
    {
        captainMeowMusic.Play();
        var limit = .4f;
        var timer = 4f;
        var maxTime = timer;

        while (timer > 0)
        {
            yield return null;
            timer -= Time.deltaTime;
            captainMeowMusic.volume = (1 - (timer / maxTime)) * limit;
            preMeowMusic.volume =  (timer / maxTime) * limit;
        }
        yield return null;
        preMeowMusic.Stop();
    }
    #endregion

    #region Unity Stuff
    private void Start()
    {
        GameManager.Instance.PauseMenuHandler.FadeTime = 3f;
        StartShipMovement();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        BeginCutscene();
    }
    #endregion
}
