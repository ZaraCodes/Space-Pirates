using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private SpacePiratesControls controls;

    [SerializeField] private Image blackFade;

    public Image BlackFade { get { return blackFade; } }

    [field: SerializeField] public float FadeTime { get; set; }
    private float fadeTimer;

    [field: SerializeField] public LoadingScreen LoadingScreen { get; set; }

    [Header("Essential Prefabs"), SerializeField] private GameObject pauseMenuPrefab;
    [SerializeField] private GameObject settingsMenuPrefab;
    // [SerializeField] private GameObject dialogsPrefab;
    [SerializeField] private GameObject gameplayDialogBoxPrefab;
    [SerializeField] private GameObject dialogOverlayPrefab;

    private GameObject pauseMenuGO;
    private GameObject settingsMenuGO;
    private Button resumeButton;

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
    private void Awake()
    {
        controls = new();

        controls.UI.Pause.performed += ctx => TogglePauseMenu(ctx);
        controls.UI.UIBack.performed += ctx => ClosePauseMenu(ctx);

        //if (!dialogsInstantiated)
        //{
        //    Instantiate(dialogsPrefab);
        //    dialogsInstantiated = true;
        //}

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

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    #endregion
}
