using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem;

/// <summary>
/// Displays the movement controls at the start of the space station
/// </summary>
public class MovementTutorial : MonoBehaviour
{
    /// <summary>Reference to the controls</summary>
    private SpacePiratesControls controls;

    /// <summary>Reference to the key up display string</summary>
    [SerializeField] private TextMeshProUGUI keyUp;
    /// <summary>Reference to the key left display string</summary>
    [SerializeField] private TextMeshProUGUI keyLeft;
    /// <summary>Reference to the key down display string</summary>
    [SerializeField] private TextMeshProUGUI keyDown;
    /// <summary>Reference to the key right display string</summary>
    [SerializeField] private TextMeshProUGUI keyRight;
    /// <summary>Reference to the control stick display string</summary>
    [SerializeField] private TextMeshProUGUI stick;
    /// <summary>GameObject that contains the buttons for the keyboard</summary>
    [Space, SerializeField] private GameObject keyboardPrompt;
    /// <summary>GameObject that contains the control stick display</summary>
    [SerializeField] private GameObject gamepadPrompt;

    /// <summary>
    /// Updates the display of the keyboard icons or the control stick
    /// </summary>
    /// <param name="inputScheme">The input scheme decides which set of control inputs will be displayed</param>
    private void UpdatePrompts(EInputScheme inputScheme)
    {
        // don't ask why tutorial doesn't remove itself from the input scheme change event. so I have to do this in order to prevent an error
        if (gamepadPrompt == null) return;

        if (inputScheme == EInputScheme.MouseKeyboard)
        {
            gamepadPrompt.SetActive(false);
            keyboardPrompt.SetActive(true);
            
            // format the string to use the icons
            string displayString = controls.Nova.Move.GetBindingDisplayString().Split("| ")[1];
            var bindingsString = displayString.ToLower().Split('/');

            // assign the binding sprites
            keyUp.text = $"<sprite name=\"Keyboard_{bindingsString[0]}\">";
            keyLeft.text = $"<sprite name=\"Keyboard_{bindingsString[1]}\">";
            keyDown.text = $"<sprite name=\"Keyboard_{bindingsString[2]}\">";
            keyRight.text = $"<sprite name=\"Keyboard_{bindingsString[3]}\">";
        }
        else
        {
            keyboardPrompt.SetActive(false);
            gamepadPrompt.SetActive(true);

            SetString(stick, controls.Nova.Move.bindings);
        }
    }

    /// <summary>
    /// Sets the control stick string
    /// </summary>
    /// <param name="buttonText">The display string that displays the stick icon</param>
    /// <param name="bindings">The bindings for the move action</param>
    private void SetString(TextMeshProUGUI buttonText, ReadOnlyArray<InputBinding> bindings)
    {
        var buttonName = InputIconStringSetter.GetIconStringFromBinding(bindings);

        buttonText.text = $"<sprite name=\"{buttonName}\">";
    }

    /// <summary>
    /// initializes the controls
    /// </summary>
    private void Awake()
    {
        controls = new();
    }

    /// <summary>
    /// Updates the prompts on startup
    /// </summary>
    private void Start()
    {
        UpdatePrompts(GameManager.Instance.CurrentInputScheme);
    }

    /// <summary>
    /// enables the controls and add itself to the input scheme changed event
    /// </summary>
    public void OnEnable()
    {
        controls.Enable();
        GameManager.Instance.OnInputSchemeChanged += inputScheme => UpdatePrompts(inputScheme);
    }

    /// <summary>
    /// disables the controls and removes itself from the input scheme changed event
    /// </summary>
    private void OnDisable()
    {
        GameManager.Instance.OnInputSchemeChanged -= inputScheme => UpdatePrompts(inputScheme);
        controls.Disable();
    }
}
