using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class DialogMessage : MonoBehaviour
{
    #region Fields
    [SerializeField] private TextMeshProUGUI characterName;
    public TextMeshProUGUI CharacterName
    {
        get { return characterName; }
        set { characterName = value; }
    }
    [SerializeField] private TextMeshProUGUI messageContent;
    [SerializeField] private Image characterPortrait;
    public Image CharacterPortrait
    {
        get { return characterPortrait; }
        set { characterPortrait = value; }
    }
    [SerializeField] private GameObject portraitFrame;
    public GameObject PortraitFrame { get { return portraitFrame; } }
    public float TextSpeedMultiplier { get; set; }

    public string VisibleText { get; set; }
    public string InvisibleText { get; set; }
    #endregion

    #region Methods
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

    public void SetMessageText()
    {
        messageContent.text = $"{VisibleText}<color #FFF0>{InvisibleText}";
    }

    /// <summary>Automatically completes the message, no matter how much the text has scrolled so far</summary>
    public void CompleteMessage()
    {
        VisibleText += InvisibleText;
        SetMessageText();
    }

    public void FillContent(ref float timer)
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
    }

    #endregion

    #region Unity Stuff

    #endregion
}
