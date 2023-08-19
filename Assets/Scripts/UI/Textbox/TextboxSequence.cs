using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Content of a conversation
/// </summary>
[CreateAssetMenu(fileName = "New Textbox Sequence", menuName = "Textbox/Textbox Sequence")]
public class TextboxSequence : ScriptableObject
{
    /// <summary>The ID of the dialog sequence</summary>
    public string ID;
    /// <summary>Array of dialog sections</summary>
    public TextboxContent[] Contents;
    /// <summary>Should this dialog sequence releat?</summary>
    public bool Repeat;
    /// <summary>This Dialog gets played when Repeat is false, to summarize the message</summary>
    public TextboxSequence SummaryDialog;
}