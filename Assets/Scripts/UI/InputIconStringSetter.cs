using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

/// <summary>
/// Static class that creates a string for a given binding
/// </summary>
public static class InputIconStringSetter
{
    /// <summary>
    /// Makes a string that can get interpreted by the tmp sprite asset that's used to display button prompts
    /// </summary>
    /// <param name="bindings">The bindings of an input action</param>
    /// <returns></returns>
    public static string GetIconStringFromBinding(ReadOnlyArray<InputBinding> bindings)
    {
        string binding;
        if (GameManager.Instance.CurrentInputScheme == EInputScheme.Gamepad)
        {
            binding = bindings[(int)EInputScheme.Gamepad].path;
        }
        else
        {
            binding = bindings[(int)EInputScheme.MouseKeyboard].path;
        }
        binding = binding.Replace("Interact:", string.Empty);
        binding = binding.Replace("<Keyboard>/", "Keyboard_");
        binding = binding.Replace("<Gamepad>/", "Gamepad_");
        binding = binding.Replace("<Mouse>/", "Mouse_");
        binding = binding.Replace('/', '_');
        
        return binding;
    }
}
