using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

/// <summary>
/// Contains information about a speaker
/// </summary>
[CreateAssetMenu(fileName = "New Speaker", menuName = "Textbox/Textbox Speaker")]
public class TextboxSpeaker : ScriptableObject
{
    /// <summary>The name of the speaker</summary>
    public LocalizedString SpeakerName;
    /// <summary>The color of their text</summary>
    public Color TextColor = Color.white;
    /// <summary>The color of their name</summary>
    public Color NameColor = Color.white;
    /// <summary>The character's portrait</summary>
    public Sprite Portrait;
}
