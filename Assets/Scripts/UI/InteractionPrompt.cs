using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;

public class InteractionPrompt : MonoBehaviour
{
    #region Fields
    [Header("Reference"), SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private RectTransform textTransform;
    [SerializeField] private RectTransform backgroundTransform;

    private Transform objectToAttachTo;
    private Camera cam;
    #endregion

    #region Methods
    /// <summary>Enables the interaction prompt</summary>
    /// <param name="text">The text that should be displayed</param>
    /// <param name="button">The input button</param>
    /// <param name="other">The transform this prompt should be attached to</param>
    public void EnablePrompt(LocalizedString text, string button, Transform other = null)
    {
        gameObject.SetActive(true);
        interactText.text = $"<sprite name=\"{button}\">{text.GetLocalizedString()}";

        if (other != null) objectToAttachTo = other;
        
        Canvas.ForceUpdateCanvases();
        backgroundTransform.sizeDelta = textTransform.sizeDelta;

        if (cam == null) cam = Camera.main;
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
    #endregion
}
