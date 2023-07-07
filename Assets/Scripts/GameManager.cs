using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get 
        {
            instance ??= new();
            return instance;
        }
    }

    /// <summary>This bool is used to enable or disable various gameplay elements depending on its value</summary>
    public bool IsPlaying { get; set; }

    /// <summary>Tracks if the settings menu is currently open</summary>
    public bool IsSettingsMenuOpen { get; set; }

    /// <summary>Reference to the gameplay dialog box of the current scene</summary>
    public GameplayDialogBox GameplayDialog { get; set; }

    public delegate void InputSchemeChanged(EInputScheme newScheme);
    public event InputSchemeChanged OnInputSchemeChanged;

    private EInputScheme currentInputScheme;
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

    private GameManager()
    {
        //Todo: Load Default Game State
        IsPlaying = true;
        IsSettingsMenuOpen = false;
        CurrentInputScheme = EInputScheme.MouseKeyboard;
    }

    public void ChangeInputScheme(InputAction.CallbackContext ctx)
    {
        if (DidInputSchemeChange(ctx))
        {
            if (CurrentInputScheme == EInputScheme.Gamepad) CurrentInputScheme = EInputScheme.MouseKeyboard;
            else if (CurrentInputScheme == EInputScheme.MouseKeyboard) CurrentInputScheme = EInputScheme.Gamepad;
        }
    }

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
