using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

/// <summary>
/// The settings menu allows the player to change settings
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    /// <summary>Reference to the main canvas scaler</summary>
    private CanvasScaler mainCanvasScaler;

    /// <summary>Reference to the controls</summary>
    private SpacePiratesControls controls;

    /// <summary>Reference to the parent menu</summary>
    [SerializeField] private GameObject parentMenu;
    /// <summary>Reference to the parent menu</summary>
    public GameObject ParentMenu
    {
        get { return parentMenu; }
        set { parentMenu = value; }
    }
    /// <summary>The button that will be selected when returning to the parent menu</summary>
    [SerializeField] private Button returnButtonToSelect;
    /// <summary>The button that will be selected when returning to the parent menu</summary>
    public Button ReturnButtonToSelect
    {
        get { return returnButtonToSelect; }
        set { returnButtonToSelect = value; }
    }
    /// <summary>The controls buttons in the top</summary>
    [Space, SerializeField] private Button controlsButton;
    /// <summary>The general settings object</summary>
    [SerializeField] private GameObject generalSettings;
    /// <summary>The control settings object</summary>
    [SerializeField] private GameObject controlSettings;
    /// <summary>The accessibility settings object</summary>
    [SerializeField] private GameObject accessibilitySettings;
    /// <summary>Event that's related to the UI Scale label when that receives a new value</summary>
    [SerializeField] private LocalizeStringEvent uiScaleLabelEvent;

    /// <summary>Reference to the audio mixer</summary>
    [Header("Audio Settings"), SerializeField] private AudioMixer mixer;
    /// <summary>reference to the display of the master volume</summary>
    [SerializeField] private TextMeshProUGUI masterVolumeDisplay;
    /// <summary>reference to the display of the music volume</summary>
    [SerializeField] private TextMeshProUGUI musicVolumeDisplay;
    /// <summary>reference to the display of the sound volume</summary>
    [SerializeField] private TextMeshProUGUI soundVolumeDisplay;
    /// <summary>reference to the master volume slider</summary>
    [SerializeField] private Slider masterVolumeSlider;
    /// <summary>reference to the music volume slider</summary>
    [SerializeField] private Slider musicVolumeSlider;
    /// <summary>reference to the sound volume slider</summary>
    [SerializeField] private Slider soundVolumeSlider;
    

    /// <summary>Shows the general settings tab</summary>
    private void ShowSettingsTab(bool general, bool controls, bool accessibility)
    {
        generalSettings.SetActive(general);
        controlSettings.SetActive(controls);
        accessibilitySettings.SetActive(accessibility);
    }

    /// <summary>Shows the General Settings tab</summary>
    public void ShowGeneralSettings()
    {
        ShowSettingsTab(true, false, false);
        masterVolumeSlider.value = SettingsS.Instance.MasterVolume;
        musicVolumeSlider.value = SettingsS.Instance.MusicVolume;
        soundVolumeSlider.value = SettingsS.Instance.SoundVolume;
    }

    /// <summary>Shows the Controls Settings tab</summary>
    public void ShowControlSettings()
    {
        ShowSettingsTab(false, true, false);
    }

    /// <summary>Shows the Accessibility Settings tab</summary>
    public void ShowAccessibilitySettings()
    {
        ShowSettingsTab(false, false, true);
    }

    /// <summary>Sets the Language</summary>
    /// <param name="index">The language index</param>
    public void SetLanguage(int index)
    {
        if (index < LocalizationSettings.AvailableLocales.Locales.Count)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
            SettingsS.Instance.CurrentLocaleIndex = index;
        }
    }

    /// <summary>Scales the UI</summary>
    /// <param name="scale">The scale factor by which the canvas will be scaled</param>
    public void ScaleCanvas(float scale)
    {
        mainCanvasScaler ??= transform.parent.GetComponent<CanvasScaler>();

        if (mainCanvasScaler != null)
        {

            //Todo: Test if scale would be too big for the current resolution 

            mainCanvasScaler.scaleFactor = scale;
            SettingsS.Instance.UIScale = scale;
            SetUiScaleLabel(scale);
        }
    }

    /// <summary>Changes the global variable that handles the display of the current UI scale</summary>
    /// <param name="scale">The new scale</param>
    private void SetUiScaleLabel(float scale)
    {
        var source = LocalizationSettings.StringDatabase.SmartFormatter.GetSourceExtension<PersistentVariablesSource>();
        var uiScale = source["global"]["uiScale"] as IntVariable;
        uiScale.Value = (int) scale;
    }

    /// <summary>
    /// Closes the settings menu
    /// </summary>
    /// <param name="ctx">Callback context of the input action</param>
    private void CloseSettingsMenu(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.IsSettingsMenuOpen = false;

        parentMenu.SetActive(true);
        gameObject.SetActive(false);
        returnButtonToSelect.Select();
    }

    /// <summary>Calculates and sets the master volume</summary>
    /// <param name="value">Slider value between .0001f and 1f</param>
    public void SetMasterVolume(float value)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        SettingsS.Instance.MasterVolume = value;
        masterVolumeDisplay.text = $"{(int) (value * 200f)}";
    }

    /// <summary>Calculates and sets the music volume</summary>
    /// <param name="value">Slider value between .0001f and 1f</param>
    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        SettingsS.Instance.MusicVolume = value;
        musicVolumeDisplay.text = $"{(int)(value * 200f)}";
    }

    /// <summary>
    /// Calculates and sets the sound volume
    /// </summary>
    /// <param name="value">Slider value between .0001f and 1f</param>
    public void SetSoundVolume(float value)
    {
        mixer.SetFloat("SoundVolume", Mathf.Log10(value) * 20);
        SettingsS.Instance.SoundVolume = value;
        soundVolumeDisplay.text = $"{(int)(value * 200f)}";
    }

    #region Unity Stuff
    /// <summary>
    /// Initializes the controls
    /// </summary>
    private void Awake()
    {
        controls = new();
        controls.UI.UIBack.performed += ctx => CloseSettingsMenu(ctx);
    }

    /// <summary>
    /// Enables the controls and opens the settings
    /// </summary>
    private void OnEnable()
    {
        controls.Enable();
        ShowGeneralSettings();
        controlsButton.Select();
        SetUiScaleLabel(SettingsS.Instance.UIScale);
    }

    /// <summary>
    /// disables the controls
    /// </summary>
    private void OnDisable()
    {
        controls.Disable();
    }
    #endregion
}
