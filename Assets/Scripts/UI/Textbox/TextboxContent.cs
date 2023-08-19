using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

/// <summary>
/// The content of a dialog message section
/// </summary>
[CreateAssetMenu(fileName = "New TextBox Content", menuName = "Textbox/Textbox Content")]
public class TextboxContent : ScriptableObject
{
    /// <summary>Data of the speaker</summary>
    public TextboxSpeaker Speaker;
    /// <summary>The text in this sequence</summary>
    public LocalizedString SequenceText;
    /// <summary>The text speed multiplier of the section</summary>
    public float TextSpeedMultiplier = 1f;
    /// <summary>
    /// True if the messages should be displayed on the left side, false if on the right.
    /// Only does something in the dialog overlay
    /// </summary>
    public bool positionedLeft = true;
    /// <summary>
    /// Unused event that's useless because I can't really assign code here
    /// </summary>
    public UnityEvent OnContentFinished;
}
