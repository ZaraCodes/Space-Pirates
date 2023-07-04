using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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

    [SerializeField] private GameObject pauseMenuPrefab;
    [SerializeField] private GameObject settingsMenuPrefab;

    private GameObject pauseMenuGO;
    private GameObject settingsMenuGO;
    private Button resumeButton;

    private void TogglePauseMenu(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPerformedThisFrame())
        {
            if (pauseMenuGO == null) InstantiatePauseMenu();

            GameManager.Instance.IsPlaying = !GameManager.Instance.IsPlaying;
            if (GameManager.Instance.IsPlaying)
            {
                if (!GameManager.Instance.IsSettingsMenuOpen)
                {
                    pauseMenuGO.SetActive(false);
                    Time.timeScale = 1f;
                }
                else
                {
                    GameManager.Instance.IsSettingsMenuOpen = false;
                    GameManager.Instance.IsPlaying = !GameManager.Instance.IsPlaying;
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
        if (ctx.action.WasPerformedThisFrame() && !GameManager.Instance.IsSettingsMenuOpen && !GameManager.Instance.IsPlaying)
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

    private void Awake()
    {
        controls = new();

        controls.UI.Pause.performed += ctx => TogglePauseMenu(ctx);
        controls.UI.UIBack.performed += ctx => ClosePauseMenu(ctx);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
