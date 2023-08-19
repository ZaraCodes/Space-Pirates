using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// To Include the Pause Menu in a scene, please do the following:
/// 1) Add this script to the canvas that contains the menu
/// 2) Drag the Pause Menu Prefab into the pauseMenu field in the inspector.
/// 3) Drag the Settings Menu Prefab into the settingMenu field in the inspector.
/// </summary>
public class PauseMenuHandler : MonoBehaviour
{
    /// <summary>Reference to the controls</summary>
    private SpacePiratesControls controls;

    /// <summary>Reference to the image that's used to fade in and out</summary>
    [SerializeField] private Image blackFade;

    /// <summary>Reference to the image that's used to fade in and out</summary>
    public Image BlackFade { get { return blackFade; } }

    /// <summary>The time it will take to fade in or out</summary>
    [field: SerializeField] public float FadeTime { get; set; }
    /// <summary>The timer that's used to fade in and out</summary>
    private float fadeTimer;

    /// <summary>Reference to the loading screen</summary>
    [field: SerializeField] public LoadingScreen LoadingScreen { get; set; }

    /// <summary>Reference to the pause menu prefab</summary>
    [Header("Essential Prefabs"), SerializeField] private GameObject pauseMenuPrefab;
    /// <summary>Reference to the settings menu prefab</summary>
    [SerializeField] private GameObject settingsMenuPrefab;
    /// <summary>Reference to the gameplay dialog prefab</summary>
    [SerializeField] private GameObject gameplayDialogBoxPrefab;
    /// <summary>Reference to the dialog overlay prefab</summary>
    [SerializeField] private GameObject dialogOverlayPrefab;

    /// <summary>Cache of the pause menu</summary>
    private GameObject pauseMenuGO;
    /// <summary>Cache of the settings menu</summary>
    private GameObject settingsMenuGO;
    /// <summary>Cache of the resume button</summary>
    private Button resumeButton;

    /// <summary>
    /// Coroutine that fades the black image in
    /// </summary>
    /// <param name="callback">Unity Event that gets triggered when the fade has finished</param>
    /// <returns></returns>
    public IEnumerator FadeIn(UnityEvent callback)
    {
        GameManager.Instance.IsFading = true;
        if (GameManager.Instance.Nova != null)
        {
            GameManager.Instance.Nova.RbVelBuffer = GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity;
        }
        blackFade.gameObject.SetActive(true);
        fadeTimer = FadeTime;
        while (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;
            blackFade.color = new(0, 0, 0, 1 - fadeTimer / FadeTime);
            yield return null;
        }
        GameManager.Instance.IsFading = false;
        callback?.Invoke();
    }

    /// <summary>
    /// Coroutine that fades the black image out
    /// </summary>
    /// <param name="callback">Unity Event that gets triggered when the fade has finished</param>
    /// <returns></returns>
    public IEnumerator FadeOut(UnityEvent callback)
    {
        GameManager.Instance.IsFading = true;
        fadeTimer = FadeTime;
        while (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;
            if (fadeTimer < 0) fadeTimer = 0;
            blackFade.color = new(0, 0, 0, fadeTimer / FadeTime);
            yield return null;
        }
        blackFade.gameObject.SetActive(false);

        if (GameManager.Instance.Nova != null)
        {
            GameManager.Instance.Nova.Fading = false;
            GameManager.Instance.Nova.GetComponent<Rigidbody2D>().velocity = GameManager.Instance.Nova.RbVelBuffer;
        }
        GameManager.Instance.IsFading = false;
        callback?.Invoke();
    }

    /// <summary>
    /// Opens or closes the pause menu
    /// </summary>
    /// <param name="ctx">Callback context of the input action</param>
    private void TogglePauseMenu(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPerformedThisFrame())
        {
            if (pauseMenuGO == null) InstantiatePauseMenu();

            GameManager.Instance.IsPauseMenuOpen = !GameManager.Instance.IsPauseMenuOpen;
            if (!GameManager.Instance.IsPauseMenuOpen)
            {
                if (!GameManager.Instance.IsSettingsMenuOpen)
                {
                    pauseMenuGO.SetActive(false);
                    Time.timeScale = 1f;
                }
                else
                {
                    GameManager.Instance.IsSettingsMenuOpen = false;
                    GameManager.Instance.IsPauseMenuOpen = !GameManager.Instance.IsPauseMenuOpen;
                }
            }
            else
            {
                Time.timeScale = 0f;
                pauseMenuGO.SetActive(true);
                resumeButton.Select();
            }
        }
    }

    /// <summary>Closes the Pause Menu</summary>
    /// <param name="ctx"></param>
    private void ClosePauseMenu(InputAction.CallbackContext ctx)
    {
        // Debug.Log($"settings: {GameManager.Instance.IsSettingsMenuOpen} playing: {GameManager.Instance.IsPlaying}");
        if (ctx.action.WasPerformedThisFrame() && !GameManager.Instance.IsSettingsMenuOpen && GameManager.Instance.IsPauseMenuOpen)
        {
            if (!ctx.control.device.name.Contains("Keyboard")) TogglePauseMenu(ctx);
        }
    }

    /// <summary>
    /// Create the pause menu object
    /// </summary>
    private void InstantiatePauseMenu()
    {
        pauseMenuGO = Instantiate(pauseMenuPrefab);
        pauseMenuGO.transform.SetParent(transform, false);

        PauseMenu pauseMenu = pauseMenuGO.GetComponent<PauseMenu>();
        resumeButton = pauseMenu.ResumeButton;

        settingsMenuGO = Instantiate(settingsMenuPrefab);
        settingsMenuGO.transform.SetParent(transform, false);

        SettingsMenu settingsMenu = settingsMenuGO.GetComponent<SettingsMenu>();
        settingsMenu.ParentMenu = pauseMenuGO.transform.GetChild(1).gameObject;
        settingsMenu.ReturnButtonToSelect = pauseMenu.SettingsButton;


        SettingsButton settingsButton = pauseMenu.SettingsButton.GetComponent<SettingsButton>();
        settingsButton.SettingsMenu = settingsMenuGO;
        settingsButton.CurrentMenu = pauseMenuGO.transform.GetChild(1).gameObject;

        pauseMenuGO.SetActive(false);
        settingsMenuGO.SetActive(false);
    }

    #region Unity Stuff
    /// <summary>
    /// When the pause menu awakes, it creates the objects for the gameplay dialog and the dialog overlay
    /// </summary>
    private void Awake()
    {
        controls = new();

        controls.UI.Pause.performed += ctx => TogglePauseMenu(ctx);
        controls.UI.UIBack.performed += ctx => ClosePauseMenu(ctx);

        GameObject gameplayDialogBoxGO = Instantiate(gameplayDialogBoxPrefab);
        gameplayDialogBoxGO.transform.SetParent(transform, false);
        gameplayDialogBoxGO.name = gameplayDialogBoxPrefab.name;
        gameplayDialogBoxGO.SetActive(false);
        
        GameplayDialogBox gameplayDialogBox = gameplayDialogBoxGO.GetComponent<GameplayDialogBox>();
        GameManager.Instance.GameplayDialog = gameplayDialogBox;

        GameObject dialogOverlayGO = Instantiate(dialogOverlayPrefab);
        dialogOverlayGO.transform.SetParent(transform, false);
        dialogOverlayGO.name = dialogOverlayPrefab.name;
        dialogOverlayGO.SetActive(false);

        DialogOverlay dialogOverlay = dialogOverlayGO.GetComponent<DialogOverlay>();
        GameManager.Instance.DialogOverlay = dialogOverlay;

        GameManager.Instance.PauseMenuHandler = this;
    }

    /// <summary>
    /// Enables the controls
    /// </summary>
    private void OnEnable()
    {
        controls.Enable();
    }

    /// <summary>
    /// Disables the controls
    /// </summary>
    private void OnDisable()
    {
        controls.Disable();
    }
    #endregion
}
