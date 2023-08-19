using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the general state of the game
/// </summary>
public class GameManager
{
    #region Fields
    /// <summary>private instance of the singleton</summary>
    private static GameManager instance;

    /// <summary>Singleton instance from the game Mananger</summary>
    public static GameManager Instance
    {
        get 
        {
            instance ??= new();
            return instance;
        }
    }

    /// <summary>This bool is used to enable or disable various gameplay elements depending on its value</summary>
    public bool IsPlaying
    {
        get
        {
            if (IsSettingsMenuOpen ||
                IsPauseMenuOpen ||
                IsDialogOverlayOpen ||
                IsSceneIntroPlaying
                ) 
                return false;
            return true;
        }
    }

    /// <summary>Tracks if the settings menu is currently open</summary>
    public bool IsSettingsMenuOpen { get; set; }
    /// <summary>Tracks if the pause menu is currently open</summary>
    public bool IsPauseMenuOpen { get; set; }
    /// <summary>Tracks if the dialog overlay is currently open</summary>
    public bool IsDialogOverlayOpen { get; set; }
    /// <summary>Tracks if the scene intro is currently playing</summary>
    public bool IsSceneIntroPlaying { get; set; }

    /// <summary>Tracks if the black fade is currently fading</summary>
    public bool IsFading { get; set; }

    /// <summary>Reference to the gameplay dialog box of the current scene</summary>
    public GameplayDialogBox GameplayDialog { get; set; }

    /// <summary>Reference to the dialog overlay of the current scene</summary>
    public DialogOverlay DialogOverlay { get; set; }

    /// <summary>Global reference to the pause menu handler</summary>
    public PauseMenuHandler PauseMenuHandler { get; set; }
    /// <summary>Global reference to Nova</summary>
    public NovaMovement Nova { get; set; }

    /// <summary>Delegate that is used for the input scheme changed event</summary>
    /// <param name="newScheme">The new input scheme</param>
    public delegate void InputSchemeChanged(EInputScheme newScheme);
    /// <summary>This event gets triggered if the input scheme changes</summary>
    public event InputSchemeChanged OnInputSchemeChanged;

    /// <summary>The current input scheme</summary>
    private EInputScheme currentInputScheme;
    /// <summary>If the current input scheme changes, the input scheme changed event will get invoked</summary>
    public EInputScheme CurrentInputScheme
    {
        get
        {
            return currentInputScheme;
        }
        set
        {
            if (currentInputScheme != value)
            {
                currentInputScheme = value;
                OnInputSchemeChanged?.Invoke(value);
            }
        }
    }
    #endregion
    /// <summary>
    /// Constructor that initializes the game state
    /// </summary>
    private GameManager()
    {
        //Todo: Load Default Game State
        IsSettingsMenuOpen = false;
        IsPauseMenuOpen = false;
        IsDialogOverlayOpen = false;

        CurrentInputScheme = EInputScheme.MouseKeyboard;
    }

    /// <summary>
    /// Updates the input scheme value if necessary
    /// </summary>
    /// <param name="ctx">callback context of the input action</param>
    public void UpdateInputScheme(InputAction.CallbackContext ctx)
    {
        if (DidInputSchemeChange(ctx))
        {
            if (CurrentInputScheme == EInputScheme.Gamepad) CurrentInputScheme = EInputScheme.MouseKeyboard;
            else if (CurrentInputScheme == EInputScheme.MouseKeyboard) CurrentInputScheme = EInputScheme.Gamepad;
        }
    }

    /// <summary>
    /// Checks if the input scheme changes
    /// </summary>
    /// <param name="ctx">callback context of the input action</param>
    /// <returns>true if the input scheme changed, false if not</returns>
    public bool DidInputSchemeChange(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device.name.Contains("Controller") || ctx.control.device.name.Contains("Gamepad")) {
            if (currentInputScheme == EInputScheme.Gamepad) return false;
            else return true;
        }
        else
        {
            if (currentInputScheme == EInputScheme.MouseKeyboard) return false;
            else return true;
        }
    }
}
