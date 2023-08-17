using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MovementTutorial : MonoBehaviour
{
    private SpacePiratesControls controls;

    [SerializeField] private TextMeshProUGUI keyUp;
    [SerializeField] private TextMeshProUGUI keyLeft;
    [SerializeField] private TextMeshProUGUI keyDown;
    [SerializeField] private TextMeshProUGUI keyRight;
    [SerializeField] private TextMeshProUGUI stick;
    [Space, SerializeField] private GameObject keyboardPrompt;
    [SerializeField] private GameObject gamepadPrompt;

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

    private void SetString(TextMeshProUGUI buttonText, ReadOnlyArray<InputBinding> bindings)
    {
        var buttonName = InputIconStringSetter.GetIconStringFromBinding(bindings);

        buttonText.text = $"<sprite name=\"{buttonName}\">";
    }

    private void Awake()
    {
        controls = new();
    }

    private void Start()
    {
        UpdatePrompts(GameManager.Instance.CurrentInputScheme);
    }
    public void OnEnable()
    {
        controls.Enable();
        GameManager.Instance.OnInputSchemeChanged += inputScheme => UpdatePrompts(inputScheme);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnInputSchemeChanged -= inputScheme => UpdatePrompts(inputScheme);
        controls.Disable();
    }
}
