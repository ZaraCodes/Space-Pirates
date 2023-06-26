using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    private CanvasScaler mainCanvasScaler;

    private SpacePiratesControls controls;


    [SerializeField] private GameObject parentMenu;
    public GameObject ParentMenu
    {
        get { return parentMenu; }
        set { parentMenu = value; }
    }
    [SerializeField] private Button returnButtonToSelect;
    public Button ReturnButtonToSelect
    {
        get { return returnButtonToSelect; }
        set { returnButtonToSelect = value; }
    }
    [Space]
    [SerializeField] private Button controlsButton;
    [SerializeField] private GameObject generalSettings;
    [SerializeField] private GameObject controlSettings;
    [SerializeField] private GameObject accessibilitySettings;
    

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
    /// <param name="index"></param>
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
        if (mainCanvasScaler != null)
        {

            //Todo: Test if scale would be too big for the current resolution 

            mainCanvasScaler.scaleFactor = scale;
            SettingsS.Instance.UIScale = scale;
        }
    }

    private void CloseSettingsMenu(InputAction.CallbackContext ctx)
    {
        print($"{ctx.control.displayName} \"{ctx.control.device.displayName}\"");
        parentMenu.SetActive(true);
        gameObject.SetActive(false);
        returnButtonToSelect.Select();
    }

    private void Awake()
    {
        controls = new();
        controls.UI.UIBack.performed += ctx => CloseSettingsMenu(ctx);
    }

    private void OnEnable()
    {
        controls.Enable();
        ShowGeneralSettings();
        controlsButton.Select();
        mainCanvasScaler = transform.parent.GetComponent<CanvasScaler>();
        GameManager.Instance.IsSettingsMenuOpen = true;
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
