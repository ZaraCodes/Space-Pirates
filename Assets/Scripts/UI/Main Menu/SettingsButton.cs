using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] private GameObject currentMenu;

    public GameObject CurrentMenu
    {
        get { return currentMenu; }
        set { currentMenu = value; }
    }
    [SerializeField] private GameObject settingsMenu;
    public GameObject SettingsMenu
    {
        get { return settingsMenu; }
        set { settingsMenu = value; }
    }

    public void OpenSettingsMenu()
    {
        settingsMenu.SetActive(true);
        currentMenu.SetActive(false);
        GameManager.Instance.IsSettingsMenuOpen = true;
    }
}
