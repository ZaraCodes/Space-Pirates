using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New Speaker", menuName = "Textbox/Textbox Speaker")]
public class TextboxSpeaker : ScriptableObject
{
    public LocalizedString SpeakerName;
    public Color TextColor = Color.white;
    public Color NameColor = Color.white;
    public Sprite Portrait;
}
