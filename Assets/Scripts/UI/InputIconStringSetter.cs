using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public static class InputIconStringSetter
{
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

        //Debug.Log(binding);
        
        return binding;
    }
}
