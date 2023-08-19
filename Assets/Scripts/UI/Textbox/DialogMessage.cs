using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is a class used for the dialog overlay
/// </summary>
public class DialogMessage : MonoBehaviour
{
    #region Fields
    /// <summary>Reference to the display string of the character speaking</summary>
    [SerializeField] private TextMeshProUGUI characterName;
    /// <summary>Reference to the display string of the character speaking</summary>
    public TextMeshProUGUI CharacterName
    {
        get { return characterName; }
        set { characterName = value; }
    }
    /// <summary>Reference to the display string text the character says</summary>
    [SerializeField] private TextMeshProUGUI messageContent;
    /// <summary>Reference to the character portrait</summary>
    [SerializeField] private Image characterPortrait;
    /// <summary>Reference to the character portrait</summary>
    public Image CharacterPortrait
    {
        get { return characterPortrait; }
        set { characterPortrait = value; }
    }
    /// <summary>
    /// Reference to the parent object of the character portrait
    /// </summary>
    [SerializeField] private GameObject portraitFrame;

    /// <summary>
    /// Reference to the parent object of the character portrait
    /// </summary>
    public GameObject PortraitFrame { get { return portraitFrame; } }
    /// <summary>Speed multiplier of the text speed. the bigger the number, the slower the text is</summary>
    public float TextSpeedMultiplier { get; set; }

    /// <summary>The text that is visible in the message</summary>
    public string VisibleText { get; set; }
    /// <summary>The text that is invisible in the message</summary>
    public string InvisibleText { get; set; }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes the message
    /// </summary>
    /// <param name="content">the string that will be displayed in the dialog</param>
    /// <param name="characterName">the name of the character</param>
    /// <param name="textColor">the color of the text</param>
    /// <param name="nameColor">the color of the name</param>
    /// <param name="textSpeedMultiplier">the text speed multiplier</param>
    /// <returns>Height of the message</returns>
    public float InitMessage(string content, string characterName, Color textColor, Color nameColor, float textSpeedMultiplier)
    {
        this.characterName.text = characterName;
        this.characterName.color = nameColor;
        messageContent.color = textColor;
        TextSpeedMultiplier = textSpeedMultiplier;

        VisibleText = string.Empty;
        InvisibleText = content;
        SetMessageText();
        Canvas.ForceUpdateCanvases();
        
        return messageContent.rectTransform.sizeDelta.y;
    }

    /// <summary>Sets the text of the dialog message</summary>
    public void SetMessageText()
    {
        messageContent.text = $"{VisibleText}<color #FFF0>{InvisibleText}";
    }

    /// <summary>Automatically completes the message, no matter how much the text has scrolled so far</summary>
    public void CompleteMessage()
    {
        VisibleText += InvisibleText;
        InvisibleText = string.Empty;
        SetMessageText();
    }

    /// <summary>
    /// Fills the text message with visible text
    /// </summary>
    /// <param name="timer">The current scroll timer. if it reaches higher than 0, a new letter gets displayed</param>
    /// <param name="buttonPrompt">The prompt of the button to proceed the dialog</param>
    /// <param name="controls">Reference to the controls</param>
    public void FillContent(ref float timer, TextMeshProUGUI buttonPrompt, SpacePiratesControls controls)
    {
        while (timer <= 0 && InvisibleText.Length > 0)
        {
            VisibleText += InvisibleText[0];
            InvisibleText = InvisibleText.Remove(0, 1);
            messageContent.text = $"{VisibleText}<color #FFF0>{InvisibleText}";
            timer += 1f / SettingsS.Instance.TextboxSpeed * TextSpeedMultiplier;
        }
        if (InvisibleText.Length != 0 && timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else if (InvisibleText.Length == 0)
        {
            buttonPrompt.gameObject.SetActive(true);
            buttonPrompt.text = $"<sprite name=\"{InputIconStringSetter.GetIconStringFromBinding(controls.UI.ProceedDialog.bindings)}\">";
        }
    }
    #endregion
}
