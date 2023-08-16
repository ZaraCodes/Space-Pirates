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

    /// <summary>Transform where the ship will be moved to in the cutscene</summary>
    [SerializeField] private Transform spawn;

    /// <summary>Reference to Captain Meow's ship</summary>
    [SerializeField] private GameObject meowShip;

    /// <summary>Tracks if captain meow has appeared in the cutscene, which would trigger a different dialog when hitting the trigger</summary>
    private bool captainMeowAppeared;

    /// <summary>Reference to the second dialog</summary>
    [SerializeField] private TextboxTrigger captainMeowAppearsDialog;

    /// <summary>Reference to the cat teleport animation object</summary>
    [SerializeField] private GameObject catTeleportAnimation;

    /// <summary>Reference to the dialog with Captain Meow</summary>
    [SerializeField] private TextboxTrigger captainMeowDialog;

    /// <summary>Reference to the dialog after the cat got teleport</summary>
    [SerializeField] private TextboxTrigger postTeleportDialog;
    #endregion

    #region Methods
    /// <summary>Starts the ship movement before the dialog</summary>
    private void StartShipMovement()
    {
        shipRigidbody.bodyType = RigidbodyType2D.Kinematic;
        shipRigidbody.velocity = startingVelocity;
    }
    
    /// <summary>Stops the ship movement</summary>
    private void StopShipMovement()
    {
        shipRigidbody.bodyType = RigidbodyType2D.Static;
    }

    /// <summary>Starts the intro cutscene</summary>
    private void BeginPreMeowDialog()
    {
        UnityEvent onFadeInFinished = new();
        onFadeInFinished.AddListener(() =>
        {
            preMeowDialog.LoadDialog();
            StopShipMovement();
        });
        StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeIn(onFadeInFinished));
    }

    /// <summary>
    /// Starts the second part of the intro cutscene where captain meow appears
    /// </summary>
    public void StartMeowSection()
    {
        captainMeowAppeared = true;

        meowShip.SetActive(true);
        shipRigidbody.gameObject.transform.position = spawn.position;
        StartShipMovement();

        UnityEvent onFadeOutFinished = new();
        onFadeOutFinished.AddListener(() => {
            captainMeowAppearsDialog.LoadDialog();
            GameManager.Instance.PauseMenuHandler.FadeTime = 3f;
            StartCoroutine(CrossFadeMusic());
        });

        GameManager.Instance.PauseMenuHandler.FadeTime = 2f;
        StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeOut(onFadeOutFinished));
    }
    
    /// <summary>Coroutine that fades the music</summary>
    /// <returns></returns>
    private IEnumerator CrossFadeMusic()
    {
        captainMeowMusic.Play();
        var limit = .4f;
        var timer = 5f;
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

    /// <summary>Starts the third dialog</summary>
    private void BeginCaptainMeowDialog()
    {
        UnityEvent onFadeInFinished = new();
        onFadeInFinished.AddListener(() =>
        {
            StopShipMovement();
            captainMeowDialog.LoadDialog();
        });
        StartCoroutine(GameManager.Instance.PauseMenuHandler.FadeIn(onFadeInFinished));
    }

    public void ShowCatTeleportAnimation()
    {
        catTeleportAnimation.SetActive(true);
        StartCoroutine(DelayPostTeleportDialog());
    }

    private IEnumerator DelayPostTeleportDialog()
    {
        yield return new WaitForSeconds(3.5f);
        postTeleportDialog.LoadDialog();
        catTeleportAnimation.SetActive(false);
    }
    #endregion

    #region Unity Stuff
    private void Start()
    {
        GameManager.Instance.PauseMenuHandler.FadeTime = 3f;
    }

    private void Update()
    {
        if (!captainMeowAppeared && GameManager.Instance.IsFading)
        {
            StartShipMovement();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (captainMeowAppeared) BeginCaptainMeowDialog();
        else BeginPreMeowDialog();
    }
    #endregion
}
