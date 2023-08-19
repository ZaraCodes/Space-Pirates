using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Only contains references to the buttons, but also saves the game
/// </summary>
public class PauseMenu : MonoBehaviour
{
    /// <summary>
    /// Reference to the button that closes the pause menu
    /// </summary>
    public Button ResumeButton;

    /// <summary>
    /// Reference to the button that saves the game
    /// </summary>
    public Button SaveButton;

    /// <summary>
    /// Reference to the button that opens the settings menu
    /// </summary>
    public Button SettingsButton;

    public void OnSaveButtonClick()
    {
        ProgressionManager.Instance.SaveProgress();
    }
}
