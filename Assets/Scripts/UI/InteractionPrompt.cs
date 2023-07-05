using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class InteractionPrompt : MonoBehaviour
{
    #region Fields
    [Header("Reference"), SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private RectTransform textTransform;
    [SerializeField] private RectTransform backgroundTransform;
    [SerializeField] private TMP_SpriteAsset buttonIcons;

    private LocalizedString promptText;
    private ReadOnlyArray<InputBinding> bindings;

    private Transform objectToAttachTo;
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

    public void SetText(LocalizedString text)
    {
        promptText = text;
        
        //todo: test for availability of button sprite
        interactText.text = $"<sprite name=\"{InputIconStringSetter.GetIconStringFromBinding(bindings)}\"> {promptText.GetLocalizedString()}";

        Canvas.ForceUpdateCanvases();
        backgroundTransform.sizeDelta = textTransform.sizeDelta;

        if (cam == null) cam = Camera.main;
    }

    public void UpdateIcon(EInputScheme scheme)
    {
        string buttonName = InputIconStringSetter.GetIconStringFromBinding(bindings);

        //Todo: test for availability of button sprite
        //bool noMatchFound = true;
        //foreach (var buttonIcon in buttonIcons.spriteInfoList)
        //{
        //    if (buttonIcon.name == buttonName)
        //    {
        //        noMatchFound = false;
        //    }
        //}
        //if (noMatchFound)
        //{
        //    interactText.text = $"[] {promptText.GetLocalizedString()}";
        //}
        //else
        interactText.text = $"<sprite name=\"{buttonName}\"> {promptText.GetLocalizedString()}";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    #endregion

    #region Unity Stuff
    private void Update()
    {
        if (objectToAttachTo != null)
        {
            transform.position = cam.WorldToScreenPoint(objectToAttachTo.position);
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.OnInputSchemeChanged += newScheme => UpdateIcon(newScheme);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnInputSchemeChanged -= newScheme => UpdateIcon(newScheme);
    }
    #endregion
}
