using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

/// <summary>
/// An Interaction Prompt that shows a text and a button icon
/// </summary>
public class InteractionPrompt : MonoBehaviour
{
    #region Fields
    /// <summary>Reference to the display text</summary>
    [Header("Reference"), SerializeField] private TextMeshProUGUI interactText;
    /// <summary>RectTransform of the text</summary>
    [SerializeField] private RectTransform textTransform;
    /// <summary>RectTransform of the background</summary>
    [SerializeField] private RectTransform backgroundTransform;
    /// <summary>Reference to the sprite asset that contains the button icons</summary>
    [SerializeField] private TMP_SpriteAsset buttonIcons;

    /// <summary>The localized text that will be displayed</summary>
    private LocalizedString promptText;
    /// <summary>The bindings of the targeted input action</summary>
    private ReadOnlyArray<InputBinding> bindings;

    /// <summary>Optional transform that tells the interaction prompt where to position itself on the canvas</summary>
    private Transform objectToAttachTo;
    /// <summary>Reference to the camera</summary>
    private Camera cam;
    #endregion

    #region Methods
    /// <summary>Enables the interaction prompt</summary>
    /// <param name="text">The text that should be displayed</param>
    /// <param name="button">The input button</param>
    /// <param name="other">The transform this prompt should be attached to</param>
    public void EnablePrompt(LocalizedString text, ReadOnlyArray<InputBinding> bindings, Transform other = null)
    {
        this.bindings = bindings;
        if (other != null) objectToAttachTo = other;
        gameObject.SetActive(true);

        SetText(text);
    }

    /// <summary>
    /// Sets the text and button sprite for the prompt
    /// </summary>
    /// <param name="text">the localized text that will be displayed</param>
    public void SetText(LocalizedString text)
    {
        promptText = text;
        
        interactText.text = $"<sprite name=\"{InputIconStringSetter.GetIconStringFromBinding(bindings)}\"> {promptText.GetLocalizedString()}";

        Canvas.ForceUpdateCanvases();
        backgroundTransform.sizeDelta = textTransform.sizeDelta;

        if (cam == null) cam = Camera.main;
    }

    /// <summary>
    /// Updates the icon for the prompt
    /// </summary>
    /// <param name="scheme"></param>
    public void UpdateIcon(EInputScheme scheme)
    {
        string buttonName = InputIconStringSetter.GetIconStringFromBinding(bindings);
        interactText.text = $"<sprite name=\"{buttonName}\"> {promptText.GetLocalizedString()}";
    }

    /// <summary>
    /// Hides the prompt
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    #endregion

    #region Unity Stuff
    /// <summary>
    /// Positions the prompt on the canvas
    /// </summary>
    private void Update()
    {
        if (objectToAttachTo != null)
        {
            transform.position = cam.WorldToScreenPoint(objectToAttachTo.position);
        }
    }

    /// <summary>adds itself to the input scheme changed event</summary>
    private void OnEnable()
    {
        GameManager.Instance.OnInputSchemeChanged += newScheme => UpdateIcon(newScheme);
    }

    /// <summary>removes itself from the input scheme changed event</summary>
    private void OnDisable()
    {
        GameManager.Instance.OnInputSchemeChanged -= newScheme => UpdateIcon(newScheme);
    }
    #endregion
}
