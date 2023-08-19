using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Button that opens the settings menu
/// </summary>
public class SettingsButton : MonoBehaviour
{
    /// <summary>Reference to the current menu</summary>
    [SerializeField] private GameObject currentMenu;

    /// <summary>Reference to the current menu</summary>
    public GameObject CurrentMenu
    {
        get { return currentMenu; }
        set { currentMenu = value; }
    }
    /// <summary>Reference to the settings menu/// </summary>
    [SerializeField] private GameObject settingsMenu;
    /// <summary>Reference to the settings menu/// </summary>
    public GameObject SettingsMenu
    {
        get { return settingsMenu; }
        set { settingsMenu = value; }
    }

    /// <summary>Opens the settings menu</summary>
    public void OpenSettingsMenu()
    {
        settingsMenu.SetActive(true);
        currentMenu.SetActive(false);
        GameManager.Instance.IsSettingsMenuOpen = true;
    }
}
